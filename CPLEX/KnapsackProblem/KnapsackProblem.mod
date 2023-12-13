/*********************************************
 * OPL 22.1.1.0 Model
 * Author: Matheus
 * Creation Date: 12 de dez de 2023 at 17:23:09
 *********************************************/

 
 int n=...;
 
 range items=1..n;
 
 int v[items]=...;
 int p[items]=...;
 
 int c=...;
 
execute PARAMS {
	cplex.epgap = 0;
}

 

// dvar int+ x[items] in 0..1;
dvar boolean x[items];
 
 maximize sum(i in items) v[i]*x[i];
 
 subject to {
   available_capacity:
   sum(i in items) x[i]*p[i] <= c;
 }