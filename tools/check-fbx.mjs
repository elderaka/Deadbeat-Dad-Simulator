// Run after both generators: node tools/check-fbx.mjs
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';

for (const path of ['Assets/House/rumah-3-petak-v01.fbx', 'Assets/Housing/kontrakan-row-5-v01.fbx']) {
  const bytes = readFileSync(path);
  assert.equal(bytes.subarray(0, 23).toString('binary'), 'Kaydara FBX Binary  \x00\x1a\x00');
  assert.equal(bytes.readUInt32LE(23), 7400);
  const nodes = new Map();
  let offset = 27;
  while (bytes.readUInt32LE(offset)) {
    const end = bytes.readUInt32LE(offset);
    assert(end > offset && end < bytes.length);
    const size = bytes[offset + 12];
    nodes.set(bytes.toString('ascii', offset + 13, offset + 13 + size), offset + 13 + size);
    offset = end;
  }
  const time = nodes.get('CreationTime');
  const id = nodes.get('FileId');
  assert(time && id, 'SDK checksum requires matching CreationTime and FileId');
  assert.equal(bytes.toString('ascii', time + 5, time + 5 + bytes.readUInt32LE(time + 1)), '1970-01-01 10:00:00:000');
  assert.equal(bytes.subarray(id + 5, id + 21).toString('hex'), '28b32aebb624ccc2bfc8b02aa92bfcf1');
  assert.equal(bytes.subarray(offset + 13, offset + 29).toString('hex'), 'fabcab09d0c8d466b176fb831cf7267e');
  assert.equal((bytes.length - 144) % 16, 0, 'Footer must be aligned');
  assert.equal(bytes.readUInt32LE(bytes.length - 140), 7400);
  console.log('FBX_CHECK_PASSED:', path);
}
