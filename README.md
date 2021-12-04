# GTKSharp Window Decorator

A C# class generator for `glade` files.

The binary is framework independant written in .NET 6. The file `gtksd`, `gtksd.exe` is the renamed output file for shorter name on invocation. You can get `gtksd` from the releases page.

## How to use
`gtksd` shouldn't be too hard to use unless you are unfamiliar with command line interface. You can always refer to built in help by invoking `gtksd -h`.

```
Usage:
  gtksd [options]

Options:
  --file <file>                      The .glade XML input file to generate from
  --window <window>                  The window id in the glade resource
  --with-namespace <with-namespace>  Namespace for generated class (optional)
  --output-folder <output-folder>    Output location for generated files (optional, defaults to current directory)
  --version                          Show version information
  -?, -h, --help                     Show help and usage information
```

### Example
```
gtksd --file abs-auto.glade --window MainWindow --with-namespace ABS --output-folder UI
```

### Sample Output
```csharp
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace ABS {

public partial class MainWindow: Window
{

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        builder.Autoconnect(this);
    }

        [UI] public Button btnSpeakSequence1
        [UI] public Label lblSequence1NomorPolisi
        [UI] public Label lblSequence1NomorPO
        [UI] public Label lblSequence1KodeMaterial
        [UI] public Label lblSequence1DocKey
        [UI] public Label lblSequence1Sequence1Start
        [UI] public Label lblSequence1Sequence2Start
        [UI] public Label lblSequence1Sequence2End
        [UI] public Label lblSequence1TipeTruk
        [UI] public Button btnSpeakSequence2
        [UI] public Label lblSequence2NomorPolisi
        [UI] public Label lblSequence2NomorPO
        [UI] public Label lblSequence2KodeMaterial
        [UI] public Label lblSequence2DocKey
        [UI] public Label lblSequence2Sequence1Start
        [UI] public Label lblSequence2Sequence2Start
        [UI] public Label lblSequence2Sequence2End
        [UI] public Label lblSequence2TipeTruk
        [UI] public Button btnReconnect
        [UI] public Button btnReset
        [UI] public Button btnConfigure

}

}
```

## TODOs
There are a lot of improvements to be made since the code was written at 2AM lol and is very messy.
- Refactor and clean up code
- Regenerate file on glade file change feature
- Compile for Linux. Shouldn't be too hard since there is no platform-specific code.

# Warnings
- `gtksd`, `GtkSharpWindowDecorator` was written with naive approach, assuming all GTK classes that should be generated has the `Gtk` prefix to be removed and might not be suitable for every cases.
- `gtksd`, `GtkSharpWindowDecorator` generates only definitions of a `GtkWindow` with the widget id specified in `--window` switch.

Pull requests, issues and discussions are welcome to improve `gtksd`.  
Have a nice day.