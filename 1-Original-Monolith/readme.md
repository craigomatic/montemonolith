# Part 1 - Understanding the Monte Carlo Simulator Monolith

### Monoliths vs Microservices

Traditionally, most applications uses monolithic architectures: front end, back end, data store. These in retrospect become difficult to scale, especially as teams and features grew. Each component were blocks you built into your app that were coupled to the allocated resources in your infrastructure. So for example to add some context, In your Front End you may have - Web services, Caching and Optimization maybe.  Backend - business logic, batch processing, and authentication and Data would be persistence. When the application was reaching peak load, you would have to increase the hardware the application was hosted on (either more RAM, CPU, disk). Consequently, I'd increased the size of the VM my application was hosted on. With cloud becoming the new standard for building scalable distributed systems, microservices are rapidly becoming the new standard to utilize the cloud to its greatest potential. You have this classic three tier model that does not necessarily map to today’s current industry key functionality: 
  - Continuous rapid deployment 
  - High Availibility
  - Intelligent scaling 
  - Resource optimization 
  
 
Here in a Microservices approach this addresses the issue - Start by breaking down monolith functionality into single, separate services. Take that Front end, decouple the Web services from caching from search engine optimization and place them behind RESTful API endpoints to divide the functionality of you app. This is a growing approach inside the industry right now. Many companies are either implementing or exploring microservices and containers. 

<img src="https://docs.microsoft.com/en-us/azure/service-fabric/media/service-fabric-overview-microservices/monolithic-vs-micro.png" width="700">

 
This makes it Easier to scale if some single component is reaching load capacity. It also becomes Faster to deploy and load balance that single service to multiple environments,allowing teams to own one entire single component or functionality. Historically with a monolithic architecture, it would take about 2 years to deploy and iterate new features for your application (Think of Windows or Adobe). With Microservices, this will help accelerate the growth of your business and time to market. 
 

Here's a side by side comparison of how monolithic services are distributed and microservices are distributed. On the monolith you have relatively large statically allocated VMs or on-prem racks to host your blocks. On the microservice application functionality is distributed across a cluster of smaller vm instances.This is Database-agnostic. In fact, microservices is pretty good at giving developers the flexibility in technologies and code language to help them build universal schemas that communicate across data store. One thing to consider when envisioning your monolith to a microservice is to identify which components in your app are stateful vs stateless. This is fundamental to the cluster manager and how it determins the logic of partitioning and reallocating your services.
 
 ### Monte Carlo Simulation Monolith

 A Monte Carlo simulation rely on repeated random sampling to obtain numerical results. Their essential idea is using randomness to solve problems that might be deterministic in principle. They are often used in physical and mathematical problems and are most useful when it is difficult or impossible to use other approaches. Monte Carlo methods are mainly used in three distinct problem classes: optimization, numerical integration, and generating draws from a probability distribution.

 From predicting Super Bowls, forecasting hurricanes, Casino card game simulator, or determining neurtino trajectories in Physics, Monte Carlo is used across a wide range of industries. In the context of this example we will treat the Monte Carlo Simulator as simply compute processing inside our Monolith. For our example, we build a basic ASP.net 4.5 Web Application that uses the refernce Monte Carlo Simulator code that was sourced from this C project code:  https://www.codeproject.com/Articles/32654/Monte-Carlo-Simulation.

```c
 public class MonteCarlo
    {
        public static double triangular(double Min, double Mode, double Max)
        {
            //   Declarations        
            double R = 0.0;
            //   Initialise     
            Random r = new Random();
            R = r.NextDouble();
            //    Triangular                        
            if (R == ((Mode - Min) / (Max - Min)))
            {
                return Mode;
            }
            else if (R < ((Mode - Min) / (Max - Min)))
            {
                return Min + Math.Sqrt(R * (Max - Min) * (Mode - Min));
            }
            else
            {
                return Max - Math.Sqrt((1 - R) * (Max - Min) * (Max - Mode));
            }
        }
        public static double[] simulate(int Total, double[] Tmin, double[] Tmod, double[] Tmax)
        {
            // Declarations
            int mlngEvals = 10000;
            int i = 0, i1 = 0, i2 = 0;
            double[] TMin = new double[Total];
            double[] TMod = new double[Total];
            double[] TMax = new double[Total];
            double[] mlngResults = new double[Total];
            double Time = 0.0;
            long lngWinner = 0;
            double Winner = 0;
            // Initialise            
            for (i = 0; i < Total; i++)
            {
                //     distribution parameters
                TMin[i] = Tmin[i];
                TMod[i] = Tmod[i];
                TMax[i] = Tmax[i];
                //     Results Array              
                mlngResults[i] = 0;
            }
            // The Tournament           
            for (i1 = 1; i1 <= mlngEvals; i1++)
            {
                //     Seed               
                lngWinner = 0;
                Winner = triangular(TMin[0], TMod[0], TMax[0]);
                //     And the Rest
                for (i2 = 1; i2 < Total; i2++)
                {
                    Time = triangular(TMin[i2], TMod[i2], TMax[i2]);
                    if (Time < Winner)
                    {
                        Winner = Time;
                        lngWinner = i2;
                    }
                }
                //     Bin
                mlngResults[lngWinner]++;
            }
            return mlngResults;
        }
    }
```

The Simulator is added to our ASP.net web app project as a framework class. The main simulation is done using triangular distribution. The method takes in 4 inputs:
- Total - the number of players for the Simulator
- Tmin - an array that represents the minimum number of wins per each player respectively
- Tmod - an array thar represents the mean number of wins per each player respectively
- Tmax - an array that represents the maximum number of wins per each player respectively

Open the Solution in Visual Studio and start the application by pressing F5.

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img1.PNG" width="700">

You will see the sample ASP.NET landing page. For our first simulator we will choose 2 players for our monte carlo simulation. This would be the parameters:

- Total - the number of players for the Simulator = 2
- Tmin - an array that represents the minimum number of wins per each player respectively = {11, 12}
- Tmod - an array thar represents the mean number of wins per each player respectively = {12.5, 13}
- Tmax - an array that represents the maximum number of wins per each player respectively = {17, 15}

> Here is what we would pass to the API:

```json 
http://localhost:59764/api/simulation/simulate/2/11,12/12.5,13/17,15 
```

> And here is our results!

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/img2.PNG" width="700">

We can then click the Chart menu item at the top of the page and see the result was stored:

<img src="https://rtwrt.blob.core.windows.net/post2-monteasf/montemonolith.png" width="700">

For more information about Monte Carlo Simulation check out this [resource](http://people.revoledu.com/kardi/tutorial/Simulation/index.html)
