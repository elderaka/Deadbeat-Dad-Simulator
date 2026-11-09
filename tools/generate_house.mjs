// Deterministic, dependency-free FBX 7.4 generator for Bapak Simulator.
// Run from repository root: node tools/generate_house.mjs
// Geometry is generated in metres (Y-up), matches Assets/House/layout.json.
import {writeFileSync, mkdirSync} from "node:fs";
function createHouseFBX(){
  const W=3.5, D=9, H=2.8, wall=0.15, part=0.10, dh=2.1;
  const front=[0,.90], lounge=[.76,.85], bedroom=[-.055,.85], bath=[1.05,.70];
  const colors={living:[.78,.65,.46],sleep:[.55,.66,.73],kitchen:[.55,.69,.62],bath:[.60,.62,.75],terrace:[.62,.62,.58],outer:[.87,.81,.72],inner:[.90,.88,.84],wood:[.40,.28,.18],roof:[.54,.29,.24]};
  const groups={}; const mats={};
  function put(name, vertices, faces, mat){
    if(!groups[name]){groups[name]={verts:[],faces:[]};mats[name]=mat}
    let o=groups[name].verts.length/3;
    groups[name].verts.push(...vertices);
    for(const f of faces)groups[name].faces.push(f.map(k=>k+o));
  }
  function block(n,xa,xb,ya,yb,za,zb,m){
    if(xb<=xa||yb<=ya||zb<=za)return;
    const v=[[xa,ya,za],[xb,ya,za],[xb,yb,za],[xa,yb,za],[xa,ya,zb],[xb,ya,zb],[xb,yb,zb],[xa,yb,zb]].flat();
    const f=[[0,2,1],[0,3,2],[4,5,6],[4,6,7],[0,1,5],[0,5,4],[1,2,6],[1,6,5],[2,3,7],[2,7,6],[3,0,4],[3,4,7]];
    put(n,v,f,m);
  }
  function wx(n,z,t,x0,x1,m,door){
    if(!door){block(n,x0,x1,0,H,z-t/2,z+t/2,m);return;}
    let a=door[0]-door[1]/2,b=door[0]+door[1]/2;
    block(n,x0,a,0,H,z-t/2,z+t/2,m);
    block(n,b,x1,0,H,z-t/2,z+t/2,m);
    block(n,a,b,dh,H,z-t/2,z+t/2,m);
  }
  function wz(n,x,t,z0,z1,m){block(n,x-t/2,x+t/2,0,H,z0,z1,m)}
  function frame(n,z,d,t){
    let [cx,w]=d,a=cx-w/2,b=cx+w/2,f=.045;
    block(n,a-f,a,0,dh+.06,z-t/2-.01,z+t/2+.01,"wood");
    block(n,b,b+f,0,dh+.06,z-t/2-.01,z+t/2+.01,"wood");
    block(n,a-f,b+f,dh,dh+.06,z-t/2-.01,z+t/2+.01,"wood");
  }
  function roofpanel(n,a,b){
    const [ax,ay]=a,[bx,by]=b,dx=bx-ax,dy=by-ay,len=Math.hypot(dx,dy),nx=dy/len*.10,ny=-dx/len*.10;
    const z0=-.3,z1=9.3;
    const v=[
      [ax,ay,z0],[bx,by,z0],[bx,by,z1],[ax,ay,z1],
      [ax+nx,ay+ny,z0],[bx+nx,by+ny,z0],[bx+nx,by+ny,z1],[ax+nx,ay+ny,z1]
    ].flat();
    put(n,v,[[0,2,1],[0,3,2],[4,5,6],[4,6,7],[0,1,5],[0,5,4],[1,2,6],[1,6,5],[2,3,7],[2,7,6],[3,0,4],[3,4,7]],"roof");
  }
  block("01_Floor_Living",-W/2,W/2,-.15,0,0,3,"living");
  block("02_Floor_Bedroom",-W/2,W/2,-.15,0,3,6,"sleep");
  block("03_Floor_Kitchen",-W/2,W/2,-.15,0,6,D,"kitchen");
  block("04_Floor_Bath_Tile",.35,W/2,0,.012,7,D,"bath");
  block("05_Terrace",-W/2,W/2,-.15,0,-1.2,0,"terrace");
  wx("06_Exterior_Front",0,wall,-W/2,W/2,"outer",front);
  wx("07_Exterior_Rear",D,wall,-W/2,W/2,"outer",null);
  wz("08_Exterior_Left",-W/2,wall,0,D,"outer");
  wz("09_Exterior_Right",W/2,wall,0,D,"outer");
  wx("10_Partition_Living_Bedroom",3,part,-W/2,W/2,"inner",lounge);
  wx("11_Partition_Bedroom_Kitchen",6,part,-W/2,W/2,"inner",bedroom);
  wz("12_Bathroom_WestWall",.35,part,7,D,"inner");
  wx("13_Bathroom_SouthWall",7,part,.35,W/2,"inner",bath);
  frame("14_Frame_FrontDoor",0,front,wall);
  frame("15_Frame_LivingBedroom",3,lounge,part);
  frame("16_Frame_BedroomKitchen",6,bedroom,part);
  frame("17_Frame_Bathroom",7,bath,part);
  roofpanel("18_Roof_Left_Removable",[-1.96,2.87],[0,3.73]);
  roofpanel("19_Roof_Right_Removable",[0,3.73],[1.96,2.87]);
  const p=(t,v)=>({t,v}),S=x=>p("S",x),I=x=>p("I",x),L=x=>p("L",x),F=x=>p("D",x),A=(t,v)=>p(t,v);
  const n=(name,props=[],children=[])=>({name,props,children});
  function prop(name,typ,sub,flags,...vals){return n("P",[S(name),S(typ),S(sub),S(flags),...vals.map(x=>typ==="int"?I(x):F(x))])}
  function ascii(s){const b=[];for(let i=0;i<s.length;i++)b.push(s.charCodeAt(i)&255);return b}
  function u32(x){let a=new Uint8Array(4);new DataView(a.buffer).setUint32(0,x,true);return [...a]}
  function i32(x){let a=new Uint8Array(4);new DataView(a.buffer).setInt32(0,x,true);return [...a]}
  function i64(x){let a=new Uint8Array(8);new DataView(a.buffer).setBigInt64(0,BigInt(x),true);return [...a]}
  function f64(x){let a=new Uint8Array(8);new DataView(a.buffer).setFloat64(0,x,true);return [...a]}
  function pack(q){
    const {t,v}=q;
    if(t==="R")return [82,...u32(v.length),...v];
    if(t==="S"){let b=ascii(v);return [83,...u32(b.length),...b]}
    if(t==="I")return [73,...i32(v)];
    if(t==="L")return [76,...i64(v)];
    if(t==="D")return [68,...f64(v)];
    if(t==="C")return [67,v?1:0];
    if(t==="d"||t==="i"){
      const b=[];for(const x of v)b.push(...(t==="d"?f64(x):i32(x)));
      return [t.charCodeAt(0),...u32(v.length),...u32(0),...u32(b.length),...b];
    }
    throw Error("unknown FBX prop type "+t);
  }
  function nodeBytes(node,start){
    const name=ascii(node.name),properties=node.props.flatMap(pack);
    let end=start+13+name.length+properties.length;
    const kids=[];
    for(const child of node.children){let packed=nodeBytes(child,end);kids.push(...packed);end+=packed.length}
    if(node.children.length || !node.props.length){kids.push(...new Array(13).fill(0));end+=13}
    return [...u32(end),...u32(node.props.length),...u32(properties.length),name.length,...name,...properties,...kids]
  }
  const objects=[],links=[],materialIds={},names=Object.keys(groups);let nextId=1000000;
  for(const [key,rgb] of Object.entries(colors)){
    let id=++nextId;materialIds[key]=id;
    objects.push(n("Material",[L(id),S("Material::"+key+"\x00\x01Material"),S("")],[
      n("Version",[I(102)]),n("ShadingModel",[S("phong")]),n("MultiLayer",[I(0)]),
      n("Properties70",[],[
        prop("DiffuseColor","Color","","A",...rgb),
        prop("DiffuseFactor","Number","","A",1.0),
        prop("AmbientColor","Color","","A",...rgb),
        prop("SpecularColor","Color","","A",0.0,0.0,0.0),
        prop("TransparencyFactor","Number","","A",0.0),
      ])
    ]));
  }
  let triangleCount=0;
  for(const name of names){
    let geomId=++nextId,modelId=++nextId,g=groups[name],normals=[],poly=[];
    for(const f of g.faces){
      triangleCount++;
      const [a,b,c]=f.map(k=>g.verts.slice(k*3,k*3+3));
      const u=b.map((k,i)=>k-a[i]),v=c.map((k,i)=>k-a[i]);
      const cross=[u[1]*v[2]-u[2]*v[1],u[2]*v[0]-u[0]*v[2],u[0]*v[1]-u[1]*v[0]];
      const magnitude=Math.hypot(...cross)||1;
      const normal=cross.map(q=>q/magnitude);
      normals.push(...normal,...normal,...normal);
      poly.push(f[0],f[2],-f[1]-1);
    }
    objects.push(n("Geometry",[L(geomId),S("Geometry::"+name+"\x00\x01Geometry"),S("Mesh")],[
      n("GeometryVersion",[I(124)]),
      // FBX is right-handed; Unity's importer mirrors X back to the source layout.
      n("Vertices",[A("d",g.verts.map((v,i)=>i%3===0?-v:v))]),
      n("PolygonVertexIndex",[A("i",poly)]),
      n("LayerElementNormal",[I(0)],[
        n("Version",[I(101)]),n("Name",[S("")]),
        n("MappingInformationType",[S("ByPolygonVertex")]),
        n("ReferenceInformationType",[S("Direct")]),
        n("Normals",[A("d",normals.map((v,i)=>i%3===0?-v:v))])
      ]),
      n("LayerElementMaterial",[I(0)],[
        n("Version",[I(101)]),n("Name",[S("")]),
        n("MappingInformationType",[S("AllSame")]),
        n("ReferenceInformationType",[S("IndexToDirect")]),
        n("Materials",[A("i",[0])])
      ]),
      n("Layer",[I(0)],[
        n("Version",[I(100)]),
        n("LayerElement",[],[n("Type",[S("LayerElementNormal")]),n("TypedIndex",[I(0)])]),
        n("LayerElement",[],[n("Type",[S("LayerElementMaterial")]),n("TypedIndex",[I(0)])])
      ])
    ]));
    objects.push(n("Model",[L(modelId),S("Model::"+name+"\x00\x01Model"),S("Mesh")],[
      n("Version",[I(232)]),
      n("Properties70",[],[
        prop("Lcl Translation","Lcl Translation","","A",0.0,0.0,0.0),
        prop("Lcl Rotation","Lcl Rotation","","A",0.0,0.0,0.0),
        prop("Lcl Scaling","Lcl Scaling","","A",1.0,1.0,1.0),
        prop("Visibility","Visibility","","A",1.0)
      ]),
      n("Shading",[p("C",true)]),n("Culling",[S("CullingOff")])
    ]));
    links.push(n("C",[S("OO"),L(geomId),L(modelId)]),
               n("C",[S("OO"),L(modelId),L(0)]),
               n("C",[S("OO"),L(materialIds[mats[name]]),L(modelId)]));
  }
  const setting=n("GlobalSettings",[],[
    n("Version",[I(1000)]),
    n("Properties70",[],[
      prop("UpAxis","int","Integer","",1),prop("UpAxisSign","int","Integer","",1),
      prop("FrontAxis","int","Integer","",2),prop("FrontAxisSign","int","Integer","",1),
      prop("CoordAxis","int","Integer","",0),prop("CoordAxisSign","int","Integer","",1),
      prop("UnitScaleFactor","double","Number","",100.0),
      prop("OriginalUnitScaleFactor","double","Number","",100.0)
    ])
  ]);
  const sceneId=++nextId;
  const nodes=[
    n("FBXHeaderExtension",[],[
      n("FBXHeaderVersion",[I(1003)]),n("FBXVersion",[I(7400)]),
      n("EncryptionType",[I(0)]),
      n("Creator",[S("Bapak Simulator procedural blockout generator")])
    ]),
    // Fixed timestamp, file ID and footer ID must agree for the FBX SDK checksum.
    n("FileId",[p("R",[0x28,0xb3,0x2a,0xeb,0xb6,0x24,0xcc,0xc2,0xbf,0xc8,0xb0,0x2a,0xa9,0x2b,0xfc,0xf1])]),
    n("CreationTime",[S("1970-01-01 10:00:00:000")]),
    setting,
    n("Documents",[],[n("Count",[I(1)]),
      n("Document",[L(sceneId),S("Scene"),S("Scene")],[n("Properties70"),n("RootNode",[L(0)])])]),
    n("References"),
    n("Definitions",[],[
      n("Version",[I(100)]),n("Count",[I(objects.length)]),
      n("ObjectType",[S("Model")],[n("Count",[I(names.length)])]),
      n("ObjectType",[S("Geometry")],[n("Count",[I(names.length)])]),
      n("ObjectType",[S("Material")],[n("Count",[I(Object.keys(colors).length)])])
    ]),
    n("Objects",[],objects),n("Connections",[],links),n("Takes",[],[n("Current",[S("")])])
  ];
  const out=ascii("Kaydara FBX Binary  \x00\x1a\x00").concat(u32(7400));
  for(const item of nodes)out.push(...nodeBytes(item,out.length));
  out.push(...new Array(13).fill(0));
  out.push(...[0xfa,0xbc,0xab,0x09,0xd0,0xc8,0xd4,0x66,0xb1,0x76,0xfb,0x83,0x1c,0xf7,0x26,0x7e]);
  out.push(...new Array((16-out.length%16)%16).fill(0));
  out.push(...u32(0),...u32(7400),...new Array(120).fill(0));
  out.push(...[0xf8,0x5a,0x8c,0x6a,0xde,0xf5,0xd9,0x7e,0xec,0xe9,0x0c,0xe3,0x75,0x8f,0x29,0x0b]);
  return {bytes:new Uint8Array(out),meshes:names.length,triangles:triangleCount,doors:{front,lounge,bedroom,bath}};
}
const house = createHouseFBX();
mkdirSync("Assets/House", {recursive:true});
writeFileSync("Assets/House/rumah-3-petak-v01.fbx",house.bytes);
console.log("Generated "+house.meshes+" meshes, "+house.triangles+" triangles ("+house.bytes.length+" bytes)");
