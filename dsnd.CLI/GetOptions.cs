﻿namespace Dsnd.CLI
{
    public class GetOptions
    {
        readonly string[] args;

        public GetOptions(string[] args)
        {
            this.args = args;
        }

        public string? FindTag(string tag) => args.FirstOrDefault(x => x == tag);

        public bool TagExist(string tag) => FindTag(tag) != null;

        public bool TryGetValue(string tag, out string? value)
        {
            value = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == tag && i + 1 < args.Length)
                {
                    if (!string.IsNullOrEmpty(args[i + 1]))
                    {
                        value = args[i + 1];
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
