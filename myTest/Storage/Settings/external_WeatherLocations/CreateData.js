// https://weather.codes/germany/

var data = ([]);
var temp0 = document.getElementsByTagName("dl")[0];
for(let i = 0; i < temp0.children.length - 1; i += 2){
    data.push({location:temp0.children[i].textContent, name:temp0.children[i + 1].textContent.replace("\n", "")});
}
