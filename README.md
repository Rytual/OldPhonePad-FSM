# OldPhonePad - Finite State Machine Implementation

```
 .-------------.
 |  .-------. |
 | |  NOKIA  | |
 | |  3310   | |
 | |_________| |
 | .-. .-. .-. |
 | |2| |3| |4| |
 | |A| |D| |G| |
 | '-' '-' '-' |
 |  FSM RULES  |
 '-------------'
```

## State Machines and SMS: A Perfect Match

Back when I was mastering T9 texting in my teenage years, I never realized I was essentially operating a finite state machine with my thumbs. Each press was a state transition, each space was a reset, and the '#' key was the accept state. This implementation makes those implicit state changes explicit.

This decoder solves the classic old phone keypad challenge using a **Finite State Machine (FSM)** approach with formal state transitions and proper separation of concerns.

## The Challenge

Decode old phone keypad input into readable text. The keypad layout:

```
1: &'(        2: abc       3: def
4: ghi        5: jkl       6: mno
7: pqrs       8: tuv       9: wxyz
*: backspace  0: space     #: send
```

### Examples

- `33#` → `E`
- `227*#` → `B` (type CA, backspace)
- `4433555 555666#` → `HELLO`
- `8 88777444666*664#` → `TURING`

## Why Finite State Machine?

This approach models the decoder as a proper state machine with three distinct states:

**States:**
- **Idle**: Not processing any key, waiting for input
- **Accumulating**: Building up presses of the same key
- **Backspacing**: Processing a backspace operation (immediately transitions back)

**Transitions:**
- Digit key while Idle → Accumulating (start new character)
- Same digit while Accumulating → Stay in Accumulating (increment count)
- Different digit while Accumulating → Commit character, start new (stay Accumulating)
- Space or special key → Commit and transition to Idle

**Pros:**
- Clear separation of state logic
- Easy to reason about behavior in each state
- Excellent for debugging (you know exactly what state you're in)
- Follows formal automata theory principles
- Extensible for more complex scenarios

**Cons:**
- More verbose than simple imperative approaches
- Slight overhead from state management
- Might be overkill for this particular problem
- Requires understanding of state machine concepts

Perfect for when you want formal correctness or plan to extend the functionality significantly. Like building a proper architecture before the codebase grows too large.

## Getting Started

### Prerequisites

- .NET 8.0 or later
- A fondness for formal methods and clean architecture

### Running the Code

```bash
# Clone the repository
git clone https://github.com/yourusername/OldPhonePad-FSM.git
cd OldPhonePad-FSM

# Build the project
dotnet build

# Run tests
dotnet test

# For verbose test output
dotnet test --logger "console;verbosity=detailed"
```

### Using the Decoder

```csharp
using OldPhonePad.FSM;

string result = OldPhonePadDecoder.OldPhonePad("4433555 555666#");
Console.WriteLine(result); // Output: HELLO
```

## Test Coverage

This project includes 40+ comprehensive unit tests covering:

- All provided examples
- Edge cases (empty input, multiple backspaces, excessive spaces)
- Single character decoding for all keys
- Cycling behavior (pressing a key more times than it has letters)
- Pause handling (spaces between same-key presses)
- Backspace operations (including backspacing empty strings)
- Special keys (symbols on key 1, space on key 0)
- **State transition tests** (verifying FSM behavior)
- Complex real-world scenarios (SOS, HELLO WORLD, etc.)
- Error handling (null input, missing send character)
- Stress tests (long inputs, alternating keys, many backspaces)

The FSM approach shines particularly in state transition tests, where we can verify the machine moves between states correctly.

## Project Structure

```
OldPhonePad-FSM/
├── src/
│   ├── OldPhonePad.cs                    # FSM decoder implementation
│   └── OldPhonePad.FSM.csproj
├── tests/
│   ├── OldPhonePadTests.cs              # Comprehensive test suite
│   └── OldPhonePad.FSM.Tests.csproj
├── .github/
│   └── workflows/
│       └── dotnet.yml                    # CI/CD pipeline
├── .gitignore
├── LICENSE
└── README.md
```

## Implementation Details

The FSM implementation defines three states:

```csharp
enum DecoderState
{
    Idle,           // Ready for new input
    Accumulating,   // Building up character presses
    Backspacing     // Handling deletion
}
```

State handlers process each input character and return the next state:
- `HandleIdleState()`: Processes input when no key is being accumulated
- `HandleAccumulatingState()`: Handles ongoing key presses and commits when needed
- `CommitCurrentKey()`: Outputs the accumulated character to the result

The main loop iterates through input, delegating to the appropriate state handler. When '#' is encountered, any pending character is committed and the loop terminates.

## State Diagram

```
       ┌─────┐  digit   ┌──────────────┐
       │Idle │─────────→│ Accumulating │
       └──┬──┘          └──────┬───────┘
          ↑                    │
          │ space/*            │ same digit
          │                    │ (increment)
          │                    ↓
          │               ┌────────┐
          └───────────────│Continue│
                          └────────┘

   different digit: commit + start new accumulation
   #: commit + terminate
```

## Extensions & Ideas

The FSM structure makes certain extensions particularly natural:

- Add a "Predicting" state for T9-style dictionary lookups
- Implement an "Error" state for invalid sequences
- Add a "Confirming" state for requiring explicit confirmation
- Create visual state transition diagrams for debugging
- Implement state history tracking for undo/redo
- Build a state machine debugger/visualizer
- Add timeout states (auto-commit after delay)

## Alternatives

If you enjoyed this approach, check out my other implementations:
- **OldPhonePad-DictionaryState**: Simple dictionary with manual state tracking
- **OldPhonePad-Grouping**: Groups consecutive digits before processing
- **OldPhonePad-OOP**: Object-oriented design with separate classes
- **OldPhonePad-RegexStack**: Regex preprocessing with stack-based evaluation

Each explores different architectural patterns and trade-offs.

## Contributing

Found a bug? Have an improvement? Feel free to open an issue or submit a pull request. The FSM structure should make it easy to add new states or transitions.

Please follow standard C# conventions and include tests for any new functionality.

## License

MIT License - see LICENSE file for details. Use it, modify it, extend the state machine. Just remember: every text message you ever sent was a computation on a state machine.

## Acknowledgments

- Iron Software for the coding challenge that inspired this FSM journey
- The field of automata theory, for giving us formal tools to reason about phone keypads
- Everyone who debugged their state machines by drawing circles and arrows on paper
- Nokia, for creating phones so reliable they probably still work today

---

Built with formal methods and a deep appreciation for state machine elegance. In an alternate universe, we all had CS degrees just from texting on old phones.

*Last updated: October 2025*
