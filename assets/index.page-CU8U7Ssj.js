import{a as u,p as l,b as x,h as o,j as i,k as s,q as m,m as d,r as _,R as c,C as b,D as f,d as g,o as w,s as v,n as C,t as p,u as h}from"./index-BfruV05g.js";const y=(e,r)=>r.attributes.slug,I=e=>["/news/",e];function N(e,r){if(e&1&&(o(0,"a",3)(1,"div",4)(2,"div",5)(3,"h2",6),i(4),s(),o(5,"p",7),i(6),g(7,"date"),s()(),o(8,"div",8)(9,"p",9),i(10),s()()()()),e&2){const n=r.$implicit;w("routerLink",v(7,I,n.attributes.slug)),d(4),C("",n.attributes.title," "),d(2),p(h(7,4,n.attributes.date,"longDate")),d(4),p(n.attributes.description)}}const D={meta:[{property:"og:title",content:"Race Element - News"}]};let F=(()=>{var e;class r{constructor(){this.posts=l(t=>t.attributes.type==="news")}ngOnInit(){this.posts.sort((t,a)=>t.attributes.date===void 0||a.attributes.date===void 0?t.attributes.date!==void 0?-1:a.attributes.title<t.attributes.title?1:a.attributes.title>t.attributes.title?-1:0:t.attributes.date<a.attributes.date?1:-1)}}return e=r,e.ɵfac=function(t){return new(t||e)},e.ɵcmp=u({type:e,selectors:[["app-news"]],standalone:!0,features:[x],decls:6,vars:0,consts:[[1,"mx-auto","rounded-lg","shadow-lg","select-none","container","max-w-4xl","px-3"],[1,"font-['Conthrax']","text-4xl","mb-1","text-center"],[1,"container","mx-auto","flex-wrap"],[3,"routerLink"],[1,"container","bg-[rgba(0,0,0,0.7)]","mb-3","hover:bg-[#191919]","hover:border-[transparent]","hover:border-l-2","rounded-br-lg","rounded-tl-xl","mx-auto","text-pretty"],[1,"container","text-gray-300","bg-[#030303]","rounded-tl-xl","pl-2","pr-2","pt-1","pb-1","border-l-2","border-[red]"],[1,"font-['Conthrax']","text-xl","md:text-2xl","pl-1","text-white"],[1,"text-xs","ml-1","mt-1","text-[rgba(255,70,0,0.8)]","mx-auto"],[1,"container","ml-3","pr-[1em]","pb-1","text-pretty"],[1,"text-sm","md:text-base","ml-1","mr-1","text-[rgba(255,255,255,0.78)]"]],template:function(t,a){t&1&&(o(0,"div",0)(1,"h1",1),i(2,"News"),s(),o(3,"div",2),m(4,N,11,9,"a",3,y),s()()),t&2&&(d(4),_(a.posts))},dependencies:[c,b,f]}),r})();export{F as default,D as routeMeta};
