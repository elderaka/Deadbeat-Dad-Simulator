import assert from 'node:assert/strict';
import {inflateSync} from 'node:zlib';

const planeTolerance = 1e-5; // FBX coordinates are in the generator's meter-scale layout.
const areaTolerance = 1e-8;

function* childNodes(bytes, offset, limit) {
  while (offset + 13 <= limit) {
    const end = bytes.readUInt32LE(offset);
    if (!end) return;
    const nameStart = offset + 13;
    const properties = nameStart + bytes[offset + 12];
    const children = properties + bytes.readUInt32LE(offset + 8);
    assert(offset < end && children <= end && end <= limit, 'Invalid FBX node bounds');
    yield {name: bytes.toString('ascii', nameStart, properties), properties, children, end};
    offset = end;
  }
}

function readArray(bytes, node, type, stride, read) {
  const offset = node.properties;
  assert.equal(bytes.toString('ascii', offset, offset + 1), type);
  const count = bytes.readUInt32LE(offset + 1);
  const encoding = bytes.readUInt32LE(offset + 5);
  const length = bytes.readUInt32LE(offset + 9);
  assert(encoding === 0 || encoding === 1, 'Unsupported FBX array encoding');
  assert.equal(offset + 13 + length, node.children, 'Invalid FBX array bounds');
  const raw = bytes.subarray(offset + 13, node.children);
  const data = encoding === 1 ? inflateSync(raw) : raw;
  assert.equal(data.length, count * stride, 'Invalid FBX array length');
  return Array.from({length: count}, (_, index) => data[read](index * stride));
}

export function readKontrakanTriangles(bytes) {
  const objects = [...childNodes(bytes, 27, bytes.length)].find(node => node.name === 'Objects');
  assert(objects, 'Missing FBX Objects node');
  const triangles = [];
  // The Kontrakan generator bakes placement into the mesh vertices and uses identity model transforms.
  for (const mesh of childNodes(bytes, objects.children, objects.end)) {
    if (mesh.name !== 'Geometry') continue;
    const nameOffset = mesh.properties + 9; // Skip the int64 geometry ID.
    assert.equal(bytes[nameOffset], 83, 'Expected an FBX geometry name');
    const nameLength = bytes.readUInt32LE(nameOffset + 1);
    const name = bytes.toString('utf8', nameOffset + 5, nameOffset + 5 + nameLength)
      .split('\x00')[0].replace(/^Geometry::/, '');
    const children = [...childNodes(bytes, mesh.children, mesh.end)];
    const verticesNode = children.find(node => node.name === 'Vertices');
    const indicesNode = children.find(node => node.name === 'PolygonVertexIndex');
    assert(verticesNode && indicesNode, `Missing mesh data for ${name}`);
    const vertices = readArray(bytes, verticesNode, 'd', 8, 'readDoubleLE');
    const indices = readArray(bytes, indicesNode, 'i', 4, 'readInt32LE');
    assert(vertices.length % 3 === 0 && vertices.every(Number.isFinite), `Invalid vertices for ${name}`);

    let points = [];
    for (const index of indices) {
      const start = (index < 0 ? -index - 1 : index) * 3;
      assert(start + 2 < vertices.length, `Invalid polygon index for ${name}`);
      points.push(vertices.slice(start, start + 3));
      if (index >= 0) continue;
      assert.equal(points.length, 3, `Expected triangles in ${name}`);
      triangles.push({name, points});
      points = [];
    }
    assert.equal(points.length, 0, `Unterminated polygon in ${name}`);
  }
  assert(triangles.length > 0, 'No triangles found in Kontrakan FBX');
  return triangles;
}

const subtract = (a, b) => a.map((value, axis) => value - b[axis]);
const dot = (a, b) => a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
const turn = (a, b, p) => (b[0] - a[0]) * (p[1] - a[1]) - (b[1] - a[1]) * (p[0] - a[0]);
const axes = [0, 1, 2];

function describeTriangle(triangle) {
  const [a, b, c] = triangle.points;
  const [ux, uy, uz] = subtract(b, a);
  const [vx, vy, vz] = subtract(c, a);
  const cross = [uy * vz - uz * vy, uz * vx - ux * vz, ux * vy - uy * vx];
  const length = Math.hypot(...cross);
  if (length <= 2 * areaTolerance) return null;

  return {
    ...triangle,
    normal: cross.map(value => value / length),
    min: axes.map(axis => Math.min(a[axis], b[axis], c[axis])),
    max: axes.map(axis => Math.max(a[axis], b[axis], c[axis]))
  };
}

// Clip one projected triangle to the three half-planes of the other.
function intersectTriangles(subject, clip) {
  const winding = Math.sign(turn(...clip));
  let polygon = subject;
  for (let edge = 0; edge < 3 && polygon.length; edge++) {
    const a = clip[edge];
    const b = clip[(edge + 1) % 3];
    const next = [];
    let previous = polygon.at(-1);
    let previousSide = winding * turn(a, b, previous);
    for (const point of polygon) {
      const side = winding * turn(a, b, point);
      if ((side >= 0) !== (previousSide >= 0)) {
        const t = previousSide / (previousSide - side);
        next.push(previous.map((value, axis) => value + t * (point[axis] - value)));
      }
      if (side >= 0) next.push(point);
      previous = point;
      previousSide = side;
    }
    polygon = next;
  }
  return polygon;
}

export function findCoplanarOverlaps(triangles) {
  const faces = triangles.map(describeTriangle).filter(Boolean);
  const overlaps = [];
  for (const [index, a] of faces.entries()) {
    const [origin] = a.points;
    const drop = axes.reduce((best, axis) =>
      Math.abs(a.normal[axis]) > Math.abs(a.normal[best]) ? axis : best);
    const keep = axes.filter(axis => axis !== drop);
    const project = points => points.map(point => keep.map(axis => point[axis] - origin[axis]));
    const projectedA = project(a.points);

    for (let other = index + 1; other < faces.length; other++) {
      const b = faces[other];
      if (axes.some(axis => a.max[axis] < b.min[axis] - planeTolerance ||
                            b.max[axis] < a.min[axis] - planeTolerance)) continue;
      const alignment = dot(a.normal, b.normal);
      if (Math.abs(alignment) < 1 - 1e-8 ||
          b.points.some(point => Math.abs(dot(a.normal, subtract(point, origin))) > planeTolerance)) continue;

      const polygon = intersectTriangles(project(b.points), projectedA);
      if (polygon.length < 3) continue;
      let doubleArea = 0;
      for (let vertex = 1; vertex < polygon.length - 1; vertex++)
        doubleArea += turn(polygon[0], polygon[vertex], polygon[vertex + 1]);
      const area = Math.abs(doubleArea / (2 * a.normal[drop]));
      if (area <= areaTolerance) continue;

      // The average of the clipped vertices lies inside the overlap.
      const center = [0, 0, 0];
      for (const point of polygon) {
        center[keep[0]] += point[0] / polygon.length;
        center[keep[1]] += point[1] / polygon.length;
      }
      center[drop] = -dot(a.normal, center) / a.normal[drop];
      overlaps.push({
        meshes: [a.name, b.name].sort(),
        sameFacing: alignment > 0,
        area,
        point: center.map((value, axis) => value + origin[axis])
      });
    }
  }
  return overlaps;
}

export function checkKontrakanOverlaps(bytes) {
  const triangles = readKontrakanTriangles(bytes);
  const overlaps = findCoplanarOverlaps(triangles);
  const groups = new Map();
  for (const overlap of overlaps.filter(item => item.sameFacing)) {
    const key = JSON.stringify(overlap.meshes);
    const group = groups.get(key) ?? {sample: overlap, count: 0};
    group.count++;
    if (overlap.area > group.sample.area) group.sample = overlap;
    groups.set(key, group);
  }

  console.log(`KONTRAKAN_OVERLAP_CHECK: ${triangles.length} triangles, ${groups.size} same-facing mesh pairs, ` +
    `${overlaps.filter(item => !item.sameFacing).length} opposite-facing triangle contacts (informational)`);
  for (const {sample, count} of groups.values()) {
    // Unity's FBX importer mirrors X back to the source layout.
    const [x, y, z] = sample.point;
    const point = [-x, y, z].map(value => value.toFixed(3)).join(', ');
    console.error(`  ${sample.meshes.join(' / ')}: ${count} triangle overlaps; ` +
      `largest ${sample.area.toFixed(6)} m² near (${point}) in layout coordinates`);
  }
  return groups.size === 0;
}
