# Aycblok

```
Start board:         Move 1:              Move 2:              Key:
. . . . . . . . .    . . . . . . . . .    . . . . . . . . .    o = Push Block
. . . . . . . . .    . . . . . . . . .    . . . . . . . . .    % = Break Block
. . . . . . . . .    . . . . . . . . .    . . . . . . . . .    # = Stop Block
. o . . . . . # .    . . . . . . . # .    . . . . . . . # .    $ = Goal
. . . . $ . . . .    . . . . $ . . . .    . . . . $ . . . . 
. . . . . . . % .    . . . . . . . % .    . . . . . . . % . 
. . . . . . . . .    . . . . . . . . .    . . . . . . . . . 
. . . . . . . . #    . o . . . . . . #    . . . . . . . o # 
. # . . . . . . .    . # . . . . . . .    . # . . . . . . . 

Move 3:              Move 4:              Move 5:
. . . . . . . . .    . . . . . . . . .    . . . . . . . . . 
. . . . . . . . .    . . . . . . . . .    . . . . . . . . . 
. . . . . . . . .    . . . . . . . . .    . . . . . . . . . 
. . . . . . . # .    . . . . . . . # .    . . . . . . . # . 
. . . . $ . . . .    . . . . $ . . o .    . . . . * . . . . 
. . . . . . . . .    . . . . . . . . .    . . . . . . . . . 
. . . . . . . o .    . . . . . . . . .    . . . . . . . . . 
. . . . . . . . #    . . . . . . . . #    . . . . . . . . # 
. # . . . . . . .    . # . . . . . . .    . # . . . . . . . 
```

## Generation Pipeline Example

The generation pipeline provides a way for multiple generation steps to be chained together. This is often easier than making manual calls to each generation step.

```GenerationPipeline.cs
// Create a dictionary of arguments to be passed to each pipeline step.
var args = new Dictionary<string, object>
{
    { "PuzzleArea", new Array2D<Cell>(20, 20) },
    { "RandomSeed", new RandomSeed(12345) },
};

// Create a pipeline
var pipeline = new Pipeline(
    new PuzzleGoalGenerator(goalSize: new Vector2DInt(2, 2)),
    new PuzzleMoveGenerator(pushBlockCount: 3, targetPushCount: 20),
    new PuzzleGarbageGenerator(targetDensity: 0.1f, breakBlockChance: 0.5f),
);

// Run the pipeline and retrieve an output
var results = pipeline.Generate(args);
var layout = results.GetOutput<PuzzleLayout>("PuzzleLayout");
```

## Generation Logger Example

The generators include logging messages that can be subscribed to by adding a delegate to the `Logger`, as shown in the below example.

```Logger.cs
// Have the messages printed to the console.
Logger.AddListener(Console.WriteLine);

// Or added to a list.
var messages = new List<string>();
Logger.AddListener(messages.Add);

// Make sure to have your objects unsubscribe from the event to prevent memory leaks.
Logger.RemoveListener(messages.Add);
```
