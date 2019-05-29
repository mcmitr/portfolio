import java.util.Scanner;
import java.io.File;

public class ColorIssue {

	// this is the main function to assign colors

	public static int[][] graphColor(int[][] g, int colorAvail[][], int bestVert, int colors[][], int V, int c)

	{
	
		//let's make sure we mark each vertex as visited, just for good measure. Since we will not 
		//have any loops in our DFS, we can just use the position(currentVertex,currentVertex) to mark it
		g[bestVert][bestVert] = 1;
		
		//we'll now see if a color is available, and swipe it from other connected
		//edges using coloravail, we will use a label for easy breaking later on
		
		outerLoop:
		for (int o =1;o<=c; o++) 
		{ 
			  //we check if the current color is available, if it is we set it under colors[][]
			  //and remove the color from availability. if it's not available, we skip to the next one
			  if(colorAvail[bestVert][o] == 1) 
			  { 
				 
				  colors[bestVert][1] = o;
				  colorAvail[bestVert][o] = 0;
			  } else {
				  continue; 
			  }
			  
			  
			  //this for loop checks for edges, and if it exists, we remove the current color from the available
			  //domain, thus using forward checking. 
			  
			  for (int b =1;b<=V; b++) 
			  { 
				//if we find an edge, we change the current colors value on our colorAvail table, we'll also 
				//double check and make sure it's not an already visited state
				if(g[bestVert][b] == 1 && g[b][b] != 1)
				{
					colorAvail[b][o] = 0;
					
					//We then are going to do a quick check using a for loop to make sure that the edge variable has 
					//available values. if not, we will move to the next color, and increment fringes, as 
					//this also indicates a color choice that ends our tree
					
					for (int d =1;d<= c; d++)
					{
						
						if (colorAvail[b][d] == 1)
						{
						
							break;
							
						} else if (colorAvail[b][d] == 0 && d == c) {
							
							continue outerLoop;
						}
					}//end for loop for checking edges
				
				}//end if statement
		  
			  }//end for loop
		  
			  //Now we will get our next vertex using MRV, meaning we are actually looking for the variable 
			  //with the fewest choices this time
			  int PrevCounter = 0; 
			  int counter = 0;
			  
			  for(int i= 1; i <= V; i++) 
			  { 
				  counter = 0;
				  //first a quick check to make sure it hasn't been visited yet on our graph
				  if (g[i][i] != 1) 
				  {
					//System.out.println("checking an edge.");
					  for(int j= 1; j <= c; j++) 
					  { 
						  if (colorAvail[i][j] == 0) 
							  counter++; 
					  } 
					  		if(counter > PrevCounter || counter == PrevCounter) 
					  		{ 
					  			PrevCounter = counter; 
					  			bestVert = i;
					  
					  		} 
					  			
					  
					  }
			  }	  
			  
			  //recursive call	       
			  colors = graphColor(g, colorAvail, bestVert, colors, V, c);
			  
			  //if we have found a solution, we don't want the recursive call to mess with the 
			  //color matrix anymore, so if all states have been colored, we return and ignore the loop. 
			  //Forward checking ensures we aren't returning bad data 
			  for(int u = 1; u<=V; u++)
			  {
				  
				  if(colors[u][1]== 0)
					  break;
				  if(colors[u][1]!= 0 && u== V)
				  {
					  
					  return colors;
				  }
			  }
			  
		}//end outer for loop
		
		//assuming we get here, no colors work for this state, or we have our solution. so we just return 
		//the current version of colors[][] and backtrack using recursion. It'll get changed by other 
		//iterations later if it's wrong. we'll also count fringes here, as any return from this line
		//means that it has no solutions, or IS the solution
		return colors;
		 
	}

	

	//just a short method to tell when no solutions are available
	public static void NoSolution() {

		System.out.println("No Solution available.");

	}

	//this method is used to display the contents of colors[][], assuming a solution is found
	public static void graphDisplay(int colors[][], int V) {
		System.out.println();
		System.out.println("Color the graph like so(variable,color):");
		for (int f = 1; f <= V; f++) {
			System.out.println("(" + f + "," + colors[f][1] + ")");
		}
		
	}

	public static void main(String[] args) throws Exception 

	{

		//method for reading the test file
		Scanner fileRead = new Scanner(System.in);
		System.out.println("Enter the name of your Test File(must be in the same directory as .jar): ");
		String fileName = fileRead.nextLine(); 
		
	   File file = new File("./"+ fileName); 
	    Scanner sc = new Scanner(file); 
	  
	    
	    int V;
	    int E;
	    int c;
	    
	    while (sc.hasNext()) 
	    {
	    	System.out.println();
	    	//variables we will need for the program
		    V = sc.nextInt();
		    E = sc.nextInt();
		    c = sc.nextInt();
		    System.out.println("Running Color Algorithm on: "+ V+ " " + E+" " +c);
	
			int[][] graph = new int[V + 1][V + 1];
	
			// small matrix to keep track of colors
			int[][] colors = new int[V + 1][2];
			
			// First, we are going to generate a matrix of values. Each Row corresponds to
			// one variable, and each
			// column will represent the edges the vertex has. For example, if there is a 1
			// in (1,3), there is an
			// edge between vertices 1 and 3. We start by filling a matrix with 0s until we
			// get the edges
			for (int i = 1; i <= V; i++) {
	
				// we go through each value of (i,j)
				for (int j = 1; j <= V; j++) {
	
					/// graph[i][j] = scan.nextInt();
					graph[i][j] = 0;
					// System.out.print(graph[i][j]);
				}
				// System.out.println();
			}
			
	
			// here we will apply every edge given to us by file
			
			for (int q = 1; q <= E; q++) {
				int i = sc.nextInt();
				int j = sc.nextInt();
	
				//System.out.println(i + " "+ j);
				// after we get the edge, we set that point in our matrix equal to 1, to show
				// the connection
				graph[i][j] = 1;
				// we also set the edge the other way, since these are bi-directional edges
				graph[j][i] = 1;
				
				
			}	

	
			// ok, now we use degree heuristic to pick our best variable using each i value.
			// We'll examine which
			// variable has the most edges(i values that are 1), and set it as the first
			// vertex. The counter variables
			// are merely used for comparing
			int PrevCounter = 0;
			int counter = 0;
			int bestVert = 0;
	
			// this for loop looks through each i value for each row, and tracks which has
			// the highest edges In the event of a tie
			// the first vertex is kept
			for (int i = 1; i <= V; i++) {
				counter = 0;
				for (int j = 0; j <= V; j++) 
				{
					if (graph[i][j] == 1)
						counter++;
				}
				if (counter > PrevCounter) 
				{
					PrevCounter = counter;
					bestVert = i;
	
				}
	
			}
	
			// this is a matrix to keep track of our available domain for each variable.
			// depending on the number of
			// colors entered
			int[][] colorAvail = new int[V + 1][c + 1];
	
			// set all colors as initially available by setting each row to value to 1
			for (int x = 1; x <= V; x++) {
				for (int y = 1; y <= c; y++) {
					colorAvail[x][y] = 1;
				}
			}

			//We'll also use an array here to keep track of our states for the fringe
			
			// prep work is all done, we can now color the graph
			colors = graphColor(graph, colorAvail, bestVert, colors, V, c);
			
			//if we get no solution, meaning one variable remains uncolored, we display it here
			for (int k = 1; k <= V; k++)
			{
				if (colors[k][1] == 0)
				{
					System.out.println();
					NoSolution();
					System.out.println();
					return;
				}
			}
			
			//otherwise we display how to color the graph
			graphDisplay(colors,V);
			System.out.println();
		}
	}//end while loop for reader
}
