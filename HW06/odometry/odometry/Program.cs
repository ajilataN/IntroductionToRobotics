using System.Globalization;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using System.Numerics;

class DifferentialDriveRobot
{
    const double WheelBase = 0.234;

    public static void Main(string[] args)
    {   // Data files included in root folder
        var path1 = "./data1.txt";
        var movements1 = ReadMovements(path1);
        var trajectory1 = ComputeTrajectory(movements1);
        var finalPose1 = trajectory1[^1];
        DrawTrajectory(trajectory1, "trajectory1.png");

        var path2 = "./data2.txt";
        var movements2 = ReadMovements(path2);
        var trajectory2 = ComputeTrajectory(movements2);
        var finalPose2 = trajectory2[^1];
        DrawTrajectory(trajectory2, "trajectory2.png");

        // Printing the computed final positions and orientations for both data sets
        Console.WriteLine($"Final Position on data1: [{finalPose1.X}, {finalPose1.Y}]");
        Console.WriteLine($"Final Orientation on data1: {finalPose1.Theta} radians");

        Console.WriteLine($"Final Position on data2: [{finalPose2.X}, {finalPose2.Y}]");
        Console.WriteLine($"Final Orientation on data2: {finalPose2.Theta} radians");


    }

    // Reads the data from the file and returns a list of tuples with the left and right wheel movements
    static List<(double Left, double Right)> ReadMovements(string filePath)
    {
        var data = new List<(double Left, double Right)>();
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(' ');
            var left = double.Parse(parts[0], CultureInfo.InvariantCulture);
            var right = double.Parse(parts[1], CultureInfo.InvariantCulture);
            data.Add((left, right));
        }
        return data;
    }

    // Computes the trajectory of the robot using the list of tuples
    static List<Pose> ComputeTrajectory(List<(double Left, double Right)> movements)
    {
        // Store the trajectory in a list of poses, icluding x, y and theta values of each point
        var trajectory = new List<Pose> { new Pose(0, 0, 0) };
        foreach (var (left, right) in movements)
        {
            var lastPose = trajectory[^1];
            // The change in the robot's heading
            var deltaTheta = (right - left) / WheelBase;
            var newTheta = lastPose.Theta + deltaTheta;

            // The distance travelled by the robot
            var distance = (left + right) / 2;

            // new coordinates of the robot
            var deltaX = distance * Math.Cos(newTheta);
            var deltaY = distance * Math.Sin(newTheta);
            var newX = lastPose.X + deltaX;
            var newY = lastPose.Y + deltaY;

            // Update the trajectory
            trajectory.Add(new Pose(newX, newY, newTheta));
        }
        return trajectory;
    }

    static void DrawTrajectory(List<Pose> trajectory, string filename)
    {
        // Image properties
        int imageWidth = 600;
        int imageHeight = 600;
        var backgroundColor = Color.White;
        var lineColor = Color.Black;
        var startColor = Color.Green;
        var endColor = Color.Red;

        using (Image image = new Image<Rgba32>(imageWidth, imageHeight))
        {
            // Calculate bounds for the trajectory
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var pose in trajectory)
            {
                minX = Math.Min(minX, (float)pose.X);
                maxX = Math.Max(maxX, (float)pose.X);
                minY = Math.Min(minY, (float)pose.Y);
                maxY = Math.Max(maxY, (float)pose.Y);
            }

            // Bounds for the image
            float rangeX = maxX - minX;
            float rangeY = maxY - minY;
            float scaleX = (imageWidth - 40) / rangeX; 
            float scaleY = (imageHeight - 40) / rangeY;
            float scale = Math.Min(scaleX, scaleY);

            // scaling and centering
            var centering = new Vector2(-minX * scale + 20, -minY * scale + 20);
   
            image.Mutate(ctx =>
            {
                ctx.Fill(backgroundColor);

                // Draw trajectory
                for (int i = 0; i < trajectory.Count - 1; i++)
                {
                    ctx.DrawLine(lineColor, 2f,
                        new PointF((float)(trajectory[i].X * scale + centering.X), (float)(imageHeight - (trajectory[i].Y * scale + centering.Y))),
                        new PointF((float)(trajectory[i + 1].X * scale + centering.X), (float)(imageHeight - (trajectory[i + 1].Y * scale + centering.Y))));
                }

                // start and end points
                var startPoint = new EllipsePolygon((float)(trajectory[0].X * scale + centering.X), (float)(imageHeight - (trajectory[0].Y * scale + centering.Y)), 5);
                var endPoint = new EllipsePolygon((float)(trajectory[^1].X * scale + centering.X), (float)(imageHeight - (trajectory[^1].Y * scale + centering.Y)), 5);
                ctx.Fill(startColor, startPoint);
                ctx.Fill(endColor, endPoint);
            });

            image.Save(filename);
        }
    }
}

record Pose(double X, double Y, double Theta);
