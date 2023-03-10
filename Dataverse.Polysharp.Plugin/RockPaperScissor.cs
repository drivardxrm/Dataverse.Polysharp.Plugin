//#1 global usings in usings.cs

//#2 file-scoped namespace
namespace Dataverse.Polysharp.Plugin;

public class RockPaperScissor : IPlugin
{
    static readonly string[] AcceptedValues = { "rock", "paper", "scissors" };

    //#3 record struct with init-only property
    public record struct Hand(string Player, string Play) 
    {
        public string Player { get; init; } = Player ?? "Player";
        public string Play { get; init; } 
            = AcceptedValues.Contains(Play) ? 
                Play : 
                throw new ArgumentException(
                    $"Argument value must be in {string.Join(",", AcceptedValues)}.", 
                    nameof(Play)
                    );
    }
    
    

    public void Execute(IServiceProvider serviceProvider)
    { 
        var context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
        
        var playersHand = new Hand((string)context.InputParameters["Player"], (string)context.InputParameters["Play"]);
        var computersHand = new Hand("Computer", GetComputerPlay());
        var result = PlayRockPaperScissor(playersHand, computersHand);

        //#4 Raw string literals. useful to write json strings
        context.OutputParameters["Result"] 
            =   $$"""
                  {
                    "PlayerName": "{{playersHand.Player}}",
                    "PlayerPlay": "{{playersHand.Play}}",
                    "ComputerPlay": "{{computersHand.Play}}",
                    "Outcome": "{{result}}"
                  }  
                """;


    }

    //#5 Pattern matching with Tuple
    public string PlayRockPaperScissor(Hand player, Hand computer) 
        => (player.Play, computer.Play) switch
            {
                ("rock", "paper") => "Rock is covered by paper. You loose.",
                ("rock", "scissors") => "Rock breaks scissors. You win.",
                ("paper", "rock") => "Paper covers rock. You win.",
                ("paper", "scissors") => "Paper is cut by scissors. You loose.",
                ("scissors", "rock") => "Scissors are broken by rock. You loose.",
                ("scissors", "paper") => "Scissors cut paper. You win.",
                (_, _) => "tie."
            };

    public string GetComputerPlay()
    {
        var random = new Random();
        return AcceptedValues[random.Next(0, AcceptedValues.Length)];
    }
}

