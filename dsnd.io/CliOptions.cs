using Utils;

namespace DsndCLI
{
    internal class CliOptions
    {
        public static string ExportTag = "-e";
        public static string GenerateTag = "-g";
        public static string TaksTag = "-t";
        public static string OverwriteTag = "-o";
        public static string TasksTag = "-t";
        public static string RecurseTag = "-r";
        public static string ExportPathTag = "-p";

        public static int GetTasks(GetOpt options)
        {
            int tasks = 1;

            if (options.TryGetValue(TasksTag, out var tasksStr))
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
