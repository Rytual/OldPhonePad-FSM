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

## The Challenge

This tackles the old phone keypad decoding problem using a Finite State Machine approach. The idea is to model the decoder with explicit states and transitions - idle, accumulating, and handling special cases.

The keypad layout:

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

## My Approach

I went with a formal FSM this time - three states with clear transitions between them. The state machine has:

**States:**
- **Idle**: Not processing anything, waiting for input
- **Accumulating**: Building up presses of the same key
- **Backspacing**: Processing a backspace (transitions back to idle)

**Transitions:**
- Digit key while idle → start accumulating
- Same digit while accumulating → keep accumulating
- Different digit → commit character, start new accumulation
- Space or special key → commit and go back to idle

**What works well:**
- Clear separation between states
- Easy to debug - you always know what state you're in
- Follows formal state machine principles
- Good for extending with more states later

**What's a bit much:**
- More verbose than simpler approaches
- Might be overkill for this problem
- Requires understanding state machines

Works okay for when you want formal correctness or plan to extend functionality. The DictionaryState version is simpler if you just need something that works.

## Getting Started

### Prerequisites

- .NET 8.0 or later

### Running the Code

```bash
# Clone the repository
git clone https://github.com/yourusername/OldPhonePad-FSM.git
cd OldPhonePad-FSM

# Build and test
dotnet build
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

The project has 45+ tests covering:
- All provided examples
- Edge cases (empty input, backspaces, spaces)
- Single character decoding for all keys
- Cycling behavior
- Pause handling
- Backspace operations
- Special keys
- State transition tests (verifying FSM behavior)
- Complex scenarios
- Error handling
- Stress tests with alternating keys and rapid state changes

The FSM approach makes it easy to test state transitions explicitly.

## Implementation Details

The FSM defines three states:

```csharp
enum DecoderState
{
    Idle,           // Ready for new input
    Accumulating,   // Building up character presses
    Backspacing     // Handling deletion
}
```

State handlers process each character:
- `HandleIdleState()`: Processes input when no key is accumulated
- `HandleAccumulatingState()`: Handles ongoing presses and commits when needed
- `CommitCurrentKey()`: Outputs the accumulated character

The main loop iterates through input, delegating to the appropriate handler. When '#' is encountered, any pending character commits and the loop ends.

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

## Project Structure

```
OldPhonePad-FSM/
├── src/
│   ├── OldPhonePad.cs                    # FSM decoder
│   └── OldPhonePad.FSM.csproj
├── tests/
│   ├── OldPhonePadTests.cs              # Test suite
│   └── OldPhonePad.FSM.Tests.csproj
├── .github/
│   └── workflows/
│       └── dotnet.yml                    # CI/CD
├── .gitignore
├── LICENSE
└── README.md
```

## Other Implementations

Check out the other approaches:
- **OldPhonePad-DictionaryState**: Simple dictionary with manual state tracking
- **OldPhonePad-Grouping**: Groups consecutive digits before processing
- **OldPhonePad-OOP**: Object-oriented design with separate classes
- **OldPhonePad-RegexStack**: Regex preprocessing with stack-based evaluation

Each has different tradeoffs.

## Fun Note

Getting the state transitions right took a few iterations. At first I had too many states, then realized I could simplify down to three. The FSM pattern is nice when you need to reason about what happens in each state separately - makes debugging easier when something goes wrong.

## License

MIT License - see LICENSE file for details.

---

*Built for the Iron Software coding challenge - October 2025*
