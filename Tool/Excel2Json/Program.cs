namespace Excel2Json;

internal class Program
{
    private static int Main(string[] args)
    {
        var controller = new Excel2JsonController(args);
        if (controller.Initialize() == false)
        {
            return -1;
        }

        if (controller.Run() == false)
        {
            return -2;
        }

        return 0;
    }
}