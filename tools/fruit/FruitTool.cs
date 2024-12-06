using System.ComponentModel;

namespace Fagkaffe.Tools.Fruit;

public static class FruitTools
{
    [Description("Returns the colour of a fruit")]
    public static string GetFruitColour([Description("Name of fruit")] string fruit)
    {
        return fruit switch
        {
            "appelsin" => "grønn",
            "eple" => "blå",
            "banan" => "rosa",
            _ => ""
        };
    }
}