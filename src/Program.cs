using System;
using Take.Blip.Client.Console;

namespace blip_presentation_assistant
{
    class Program
    {
        static int Main(string[] args) => ConsoleRunner.RunAsync(args).GetAwaiter().GetResult();
    }
}