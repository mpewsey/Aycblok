# Aycblok

Aycblok - pronounced "ice block," but cooler - is an ice push block puzzle generator. These puzzles are similar to the ice platform puzzles of the Ice Cavern of _Zelda: Ocarina of Time_ or the orb puzzles of _Chained Echoes_.

The aim of the puzzles is for a player to slide all push blocks onto a goal tile. However, once pushed, the push blocks will slide, frictionlessly as though on ice, until they are stopped by an obstacle: either a permanent stop block or a break block, which will be removed when hit.

The following is a move-by-move report as acquired for one such generated puzzle:

```
Start board:         Move 1:              Move 2:              Key:
. . . . . . . . .    . . . . . . . . .    . . . . . . . . .    o = Push Block
. . . . . . . . .    . . . . . . . . .    . . . . . . . . .    % = Break Block
. . . . . . . . .    . . . . . . . . .    . . . . . . . . .    # = Stop Block
. o . . . . . # .    . . . . . . . # .    . . . . . . . # .    $ = Goal
. . . . $ . . . .    . . . . $ . . . .    . . . . $ . . . .    * = Push Block at Goal
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

## Features

* Ice push block puzzle generation for a tile area.
* Input tile areas can have their cells flagged to add confining geometry to add challenge or visual interest.
* Support of multiple push blocks in a single puzzle area, which can interact and stop or block each other.
* Support of area obstacles such a holes by means of void tiles, which disallow traversal of a cell by the player, the push blocks, or both.
* Includes a random garbage block generator to clutter the area and make the solution path less obvious.
* Customizable parameters for generation steps to help adjust the feel and difficulty of puzzles.

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
