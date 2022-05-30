namespace TaskDemo
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TaskDemo1
    {
        public string ApplyBlackAndWhiteFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }
            Console.WriteLine($"[{path}] Applied B&W filter");

            return path + "_bw";
        }

        public string ApplySketchFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }
            Console.WriteLine($"[{path}] Applied Sketch filter");

            return path + "_bw";
        }

        public void ProcessImage(string path)
        {
            string bwFilterPath = ApplyBlackAndWhiteFilter(path);
            string sketchFilterPath = ApplySketchFilter(path);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"[{path}] B&W filter path: {bwFilterPath}");
            Console.WriteLine($"[{path}] Sketch filter path: {sketchFilterPath}");
        }

        public void Demo()
        {
            ProcessImage("image1path");
        }
    }

    public class TaskDemo2
    {
        public string ApplyBlackAndWhiteFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied B&W filter");

            return path + "_bw";
        }

        public string ApplySketchFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied Sketch filter");

            return path + "_bw";
        }

        public void ProcessImage(string path)
        {
            string bwFilterPath = null;
            var t1 = new Thread(() => bwFilterPath = ApplyBlackAndWhiteFilter(path));

            string sketchFilterPath = null;
            var t2 = new Thread(() => sketchFilterPath = ApplySketchFilter(path));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine(string.Empty);
            Console.WriteLine($"[{path}] B&W filter path: {bwFilterPath}");
            Console.WriteLine($"[{path}] Sketch filter path: {sketchFilterPath}");
        }

        public void Demo()
        {
            ProcessImage("image1path");
        }
    }

    public class TaskDemo3
    {
        public string ApplyBlackAndWhiteFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied B&W filter");

            return path + "_bw";
        }

        public string ApplySketchFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied Sketch filter");

            return path + "_bw";
        }

        public void ProcessImage(string path)
        {
            string bwFilterPath = null;
            var t1 = new Thread(() => bwFilterPath = ApplyBlackAndWhiteFilter(path));

            string sketchFilterPath = null;
            var t2 = new Thread(() => sketchFilterPath = ApplySketchFilter(path));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine(string.Empty);
            Console.WriteLine($"[{path}] B&W filter path: {bwFilterPath}");
            Console.WriteLine($"[{path}] Sketch filter path: {sketchFilterPath}");
        }

        public void Demo()
        {
            var t1 = new Thread(() => ProcessImage("image1path"));
            var t2 = new Thread(() => ProcessImage("image2path"));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }
    }

    public class TaskDemo4
    {
        public async Task<string> ApplyBlackAndWhiteFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied B&W filter");

            return path + "_bw";
        }

        public async Task<string> ApplySketchFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied Sketch filter");

            return path + "_bw";
        }

        public async Task ProcessImage(string path)
        {
            string bwFilterPath = await ApplyBlackAndWhiteFilter(path);
            string sketchFilterPath = await ApplySketchFilter(path);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"[{path}] B&W filter path: {bwFilterPath}");
            Console.WriteLine($"[{path}] Sketch filter path: {sketchFilterPath}");
        }

        public async Task Demo()
        {
            await ProcessImage("image1path");
        }
    }

    public class TaskDemo5
    {
        public async Task<string> ApplyBlackAndWhiteFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied B&W filter");

            return path + "_bw";
        }

        public async Task<string> ApplySketchFilter(string path)
        {
            int j = 0;
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    j = j * i;
                }
            }

            Console.WriteLine($"[{path}] Applied Sketch filter");

            return path + "_bw";
        }

        public async Task ProcessImage(string path)
        {
            var bwFilterPathTask = ApplyBlackAndWhiteFilter(path);
            var sketchFilterPathTask = ApplySketchFilter(path);

            await Task.WhenAll(bwFilterPathTask, sketchFilterPathTask);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"[{path}] B&W filter path: {bwFilterPathTask.Result}");
            Console.WriteLine($"[{path}] Sketch filter path: {sketchFilterPathTask.Result}");
        }

        public async Task Demo()
        {
            await ProcessImage("image1path");
        }
    }

    public class TaskDemo6
    {
        public async Task<string> ApplyBlackAndWhiteFilter(string path)
        {
            return await Task.Run(() =>
            {
                int j = 0;
                for (int k = 0; k < 100; k++)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        j = j * i;
                    }
                }

                Console.WriteLine($"[{path}] Applied B&W filter");

                return path + "_bw";
            });
        }

        public async Task<string> ApplySketchFilter(string path)
        {
            return await Task.Run(() =>
            {
                int j = 0;
                for (int k = 0; k < 100; k++)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        j = j * i;
                    }
                }

                Console.WriteLine($"[{path}] Applied Sketch filter");

                return path + "_bw";
            });
        }

        public async Task ProcessImage(string path)
        {
            var bwFilterPathTask = ApplyBlackAndWhiteFilter(path);
            var sketchFilterPathTask = ApplySketchFilter(path);

            await Task.WhenAll(bwFilterPathTask, sketchFilterPathTask);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"[{path}] B&W filter path: {bwFilterPathTask.Result}");
            Console.WriteLine($"[{path}] Sketch filter path: {sketchFilterPathTask.Result}");
        }

        public async Task Demo()
        {
            await ProcessImage("image1path");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide demo number");
                return;
            }

            var parseResult = int.TryParse(args[0], out int demoNumber);
            if (!parseResult)
            {
                Console.WriteLine("Please provide an integer demo number");
                return;
            }

            if (demoNumber == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    new TaskDemo1().Demo();
                    Console.WriteLine("\r\n-----\r\n");
                }
            }
            else if (demoNumber == 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    new TaskDemo2().Demo();
                    Console.WriteLine("\r\n-----\r\n");
                }
            }
            else if (demoNumber == 3)
            {
                for (int i = 0; i < 10; i++)
                {
                    new TaskDemo3().Demo();
                    Console.WriteLine("\r\n-----\r\n");
                }
            }
            else if (demoNumber == 4)
            {
                for (int i = 0; i < 10; i++)
                {
                    new TaskDemo4().Demo().Wait();
                    Console.WriteLine("\r\n-----\r\n");
                }
            }
            else if (demoNumber == 5)
            {
                for (int i = 0; i < 10; i++)
                {
                    new TaskDemo5().Demo().Wait();
                    Console.WriteLine("\r\n-----\r\n");
                }
            }
            else if (demoNumber == 6)
            {
                for (int i = 0; i < 10; i++)
                {
                    new TaskDemo6().Demo().Wait();
                    Console.WriteLine("\r\n-----\r\n");
                }
            }
        }
    }
}
