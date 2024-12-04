using System.ComponentModel;

namespace Fagkaffe.Tools.Fruit;

public static class FruitTools
{
    [Description("Returnerer fargen på en frukt")]
    public static string GetFruitColour([Description("Navn på frukt")] string fruit)
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