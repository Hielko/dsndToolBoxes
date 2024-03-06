using Utils;

namespace Dsnd.CLI
{
    internal class CliOptions
    {
        public static string ExportTag_e = "-e";
        public static string GenerateTag_g = "-g";
        public static string OverwriteTag_o = "-o";
        public static string TasksTag_t = "-t";
        public static string RecursiveTag_r = "-r";
        public static string ExportPathTag_p = "-p";

        public static int GetTasks(GetOptions options)
        {
            int tasks = 1;

            if (options.TryGetValue(TasksTag_t, out var tasksStr))
            {
                if (int.TryParse(tasksStr, out tasks))
                {
                    if (tasks < 2)
                        throw new Exception("tasks must be absent or > 1");
                    if (tasks > 16)
                        throw new Exception("tasks cant be more than 16");
                }
            }
            return tasks;
        }

    }
}
