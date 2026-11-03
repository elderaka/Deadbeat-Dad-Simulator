// Bapak Simulator five-unit kontrakan complex: deterministic, dependency-free FBX generator.
// Run in project root: node tools/generate_kontrakan_complex.mjs
import {writeFileSync,mkdirSync} from "node:fs";
function createComplexFBX(){
  const W=3.5, D=9, H=2.8, wall=0.15, part=0.10, dh=2.1;
  const front=[0,.90], lounge=[.76,.85], bedroom=[-.055,.85], bath=[1.05,.70];
  const colors={living:[.78,.65,.46],sleep:[.55,.66,.73],kitchen:[.55,.69,.62],bath:[.60,.62,.75],terrace:[.62,.62,.58],outer:[.87,.81,.72],inner:[.90,.88,.84],wood:[.40,.28,.18],roof:[.54,.29,.24],facade1:[.90,.75,.52],facade2:[.64,.79,.67],facade3:[.86,.67,.66],facade4:[.65,.73,.85],facade5:[.76,.74,.70],court:[.65,.65,.61],green:[.34,.52,.35],metal:[.35,.38,.42],line:[.96,.95,.81],soil:[.45,.40,.33],utility:[.52,.60,.67]};
  const groups={}; const mats={}; let unitOffset=0,unitPrefix='';
  function put(name, vertices, faces, mat){
    name=unitPrefix+name;
    vertices=vertices.map((v,i)=>i%3===0?v+unitOffset:v);
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

  // Exactly five row-houses, with their original interior passage positions preserved.
  for(let i=0;i<5;i++){
    unitOffset=(i-2)*W; unitPrefix="U"+String(i+1).padStart(2,"0")+"_";
  block("01_Floor_Living",-W/2,W/2,-.15,0,0,3,"living");
  block("02_Floor_Bedroom",-W/2,W/2,-.15,0,3,6,"sleep");
  block("03_Floor_Kitchen",-W/2,W/2,-.15,0,6,D,"kitchen");
  block("04_Floor_Bath_Tile",.35,W/2,0,.012,7,D,"bath");
  block("05_Terrace",-W/2,W/2,-.15,0,-1.2,0,"terrace");
  wx("06_Exterior_Front",0,wall,-W/2,W/2,"facade"+(i+1),front);
  wx("07_Exterior_Rear",D,wall,-W/2,W/2,"outer",null);
  if(i===0) wz("08_Exterior_Left",-W/2,wall,0,D,"outer");
  wz("09_Exterior_Right",W/2,wall,0,D,"outer");
  wx("10_Partition_Living_Bedroom",3,part,-W/2,W/2,"inner",lounge);
  wx("11_Partition_Bedroom_Kitchen",6,part,-W/2,W/2,"inner",bedroom);
  wz("12_Bathroom_WestWall",.35,part,7,D,"inner");
  wx("13_Bathroom_SouthWall",7,part,.35,W/2,"inner",bath);
  frame("14_Frame_FrontDoor",0,front,wall);
  frame("15_Frame_LivingBedroom",3,lounge,part);
  frame("16_Frame_BedroomKitchen",6,bedroom,part);
  frame("17_Frame_Bathroom",7,bath,part);



  }
  unitOffset=0;unitPrefix="";
  // The 19.5m x 17m plot has a 5.3m court, 1.2m terraces and 1.5m rear strip.
  // Z=0 is each house facade, Z increasing to rear.
  block("Shared_Court",-9.75,9.75,-.22,-.16,-6.5,-1.2,"court");
  block("Shared_Rear_Service",-9.75,9.75,-.22,-.16,9,10.5,"terrace");
  block("Side_Alley_Left",-9.75,-8.75,-.22,-.16,-1.2,9,"terrace");
  block("Side_Alley_Right",8.75,9.75,-.22,-.16,-1.2,9,"terrace");
  // Shared boundary with 3.2-m center entry; no gate leaf so it remains walkable.
  block("Fence_Left",-9.75,-9.62,0,1.60,-6.5,10.5,"outer");
  block("Fence_Right",9.62,9.75,0,1.60,-6.5,10.5,"outer");
  block("Fence_Rear",-9.75,9.75,0,1.60,10.37,10.5,"outer");
  block("Fence_Front_Left",-9.75,-1.60,0,1.40,-6.5,-6.37,"outer");
  block("Fence_Front_Right",1.60,9.75,0,1.40,-6.5,-6.37,"outer");
  block("Gate_Pillar_Left",-1.80,-1.60,0,1.86,-6.6,-6.25,"outer");
  block("Gate_Pillar_Right",1.60,1.80,0,1.86,-6.6,-6.25,"outer");
  block("Court_Drain",-9.4,9.4,-.15,-.12,-1.39,-1.19,"metal");
  for(let j=0;j<4;j++){
    const x=-8.80+j*1.12;
    block("Parking_Bay_"+(j+1)+"_StripeA",x,x+.045,-.12,-.11,-5.8,-3.5,"line");
    block("Parking_Bay_"+(j+1)+"_StripeB",x,x+1.0,-.12,-.11,-5.8,-5.755,"line");
  }
  // Small communal landmark: planter/tree and two sitting benches.
  block("Social_Planter_Rim",-.55,.55,-.12,.40,-4.9,-3.8,"outer");
  block("Social_Planter_Soil",-.48,.48,.40,.42,-4.83,-3.87,"soil");
  block("Social_Tree_Trunk",-.12,.12,.42,2.45,-4.45,-4.25,"wood");
  block("Social_Tree_Canopy",-1,1,2.20,3.12,-5.15,-3.42,"green");
  for(const [id,x] of [[1,-2.20],[2,1.30]]){
    block("Social_Bench_"+id+"_Seat",x,x+.91,.45,.53,-4.48,-4.02,"wood");
    block("Social_Bench_"+id+"_Leg",x+.12,x+.20,0,.45,-4.44,-4.08,"metal");
    block("Social_Bench_"+id+"_Leg",x+.73,x+.81,0,.45,-4.44,-4.08,"metal");
  }
  // Utility, washing, drying and water tank placeholders on the courtyard's right side.
  block("Utility_Slab",6.15,8.65,-.15,-.08,-5.85,-2.85,"terrace");
  block("Utility_Water_Tank",6.50,7.38,-.08,1.55,-3.75,-2.85,"utility");
  block("Utility_Sink",8.0,8.57,-.08,.80,-3.52,-2.97,"utility");
  block("Utility_Laundry_PostA",6.43,6.52,-.08,1.92,-5.55,-5.46,"metal");
  block("Utility_Laundry_PostB",8.45,8.54,-.08,1.92,-5.55,-5.46,"metal");
  block("Utility_Laundry_Rail",6.43,8.54,1.84,1.90,-5.55,-5.46,"metal");
  // Continuous roof ridge runs left-to-right across the five units.
  // Independent meshes for Unity roof visibility control.
  function complexRoof(name,z0,z1,y0,y1){
    const left=-9.02,right=9.02,thickness=.10;
    const v=[left,y0,z0,right,y0,z0,right,y1,z1,left,y1,z1,
             left,y0-thickness,z0,right,y0-thickness,z0,right,y1-thickness,z1,left,y1-thickness,z1];
    const f=[[0,2,1],[0,3,2],[4,5,6],[4,6,7],[0,1,5],[0,5,4],[1,2,6],[1,6,5],[2,3,7],[2,7,6],[3,0,4],[3,4,7]];
    put(name,v,f,"roof");
  }
  complexRoof("Roof_Front_Removable",-.4,4.5,2.85,3.70);
  complexRoof("Roof_Rear_Removable",4.5,9.4,3.70,2.85);
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
    for(const child of node.children){let packed=nodeBytes(child,end);for(const byte of packed)kids.push(byte);end+=packed.length}
    if(node.children.length){kids.push(...new Array(13).fill(0));end+=13}
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
      poly.push(f[0],f[1],-f[2]-1);
    }
    objects.push(n("Geometry",[L(geomId),S("Geometry::"+name+"\x00\x01Geometry"),S("Mesh")],[
      n("GeometryVersion",[I(124)]),
      n("Vertices",[A("d",g.verts)]),
      n("PolygonVertexIndex",[A("i",poly)]),
      n("LayerElementNormal",[I(0)],[
        n("Version",[I(101)]),n("Name",[S("")]),
        n("MappingInformationType",[S("ByPolygonVertex")]),
        n("ReferenceInformationType",[S("Direct")]),
        n("Normals",[A("d",normals)])
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
      prop("FrontAxis","int","Integer","",2),prop("FrontAxisSign","int","Integer","",-1),
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
  for(const item of nodes){const b=nodeBytes(item,out.length);for(const byte of b)out.push(byte)}
  out.push(...new Array(13).fill(0));
  out.push(...[0xfa,0xbc,0xab,0x09,0xd0,0xc8,0xd4,0x66,0xb1,0x76,0xfb,0x83,0x1c,0xf7,0x26,0x7e]);
  out.push(...u32(0),...u32(7400),...new Array(120).fill(0));
  out.push(...[0xf8,0x5a,0x8c,0x6a,0xde,0xf5,0xd9,0x7e,0xec,0xe9,0x0c,0xe3,0x75,0x8f,0x29,0x0b]);
  return {bytes:new Uint8Array(out),meshes:names.length,triangles:triangleCount,doors:{front,lounge,bedroom,bath},names};
}
const model=createComplexFBX();
mkdirSync("Assets/Housing",{recursive:true});
writeFileSync("Assets/Housing/kontrakan-row-5-v01.fbx",model.bytes);
console.log("Generated",model.meshes,"meshes, ",model.triangles,"triangles,",model.bytes.length,"bytes");
