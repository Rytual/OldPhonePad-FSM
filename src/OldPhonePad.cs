using System;
using System.Text;

namespace OldPhonePad.FSM
{
    /// <summary>
    /// Decodes old T9-style phone keypad input using a Finite State Machine approach.
    /// State transitions handle the flow from idle to accumulating presses to outputting characters.
    /// </summary>
    public static class OldPhonePadDecoder
    {
        /// <summary>
        /// Represents the various states the decoder can be in while processing input.
        /// </summary>
        private enum DecoderState
        {
            Idle,           // Not currently processing any key
            Accumulating,   // Accumulating presses of the same key
            Backspacing     // Processing a backspace operation
        }

        // The keypad mapping - brings back memories of texting in the 2000s
        private static readonly Dictionary<char, string> KeypadMap = new()
        {
            { '1', "&'(" },
            { '2', "abc" },
            { '3', "def" },
            { '4', "ghi" },
            { '5', "jkl" },
            { '6', "mno" },
            { '7', "pqrs" },
            { '8', "tuv" },
            { '9', "wxyz" },
            { '0', " " }
        };

        /// <summary>
        /// Decodes T9-style phone keypad input into readable text using a state machine.
        /// </summary>
        /// <param name="input">Input string with digits, spaces, '*' (backspace), and '#' (send).</param>
        /// <returns>The decoded text string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ArgumentException">Thrown when input doesn't end with '#'.</exception>
        public static string OldPhonePad(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            if (!input.Contains('#'))
            {
                throw new ArgumentException("Input must contain the send character '#'.", nameof(input));
            }

            var result = new StringBuilder();
            var state = DecoderState.Idle;
            char currentKey = '\0';
            int pressCount = 0;

            foreach (char c in input)
            {
                switch (state)
                {
                    case DecoderState.Idle:
                        state = HandleIdleState(c, ref currentKey, ref pressCount, result);
                        break;

                    case DecoderState.Accumulating:
                        state = HandleAccumulatingState(c, ref currentKey, ref pressCount, result);
                        break;

                    case DecoderState.Backspacing:
                        // Backspace state immediately transitions back
                        state = HandleIdleState(c, ref currentKey, ref pressCount, result);
                        break;
                }

                // Terminal state - we hit send
                if (c == '#')
                {
                    break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Handles the Idle state where we're not currently accumulating any key presses.
        /// </summary>
        private static DecoderState HandleIdleState(char c, ref char currentKey, ref int pressCount, StringBuilder result)
        {
            if (c == '#')
            {
                // Terminal - stay idle but caller will break
                return DecoderState.Idle;
            }

            if (c == '*')
            {
                // Backspace when idle
                if (result.Length > 0)
                {
                    result.Length--;
                }
                return DecoderState.Idle;  // Stay idle after backspace
            }

            if (c == ' ')
            {
                // Space while idle - just stay idle
                return DecoderState.Idle;
            }

            if (char.IsDigit(c))
            {
                // Start accumulating a new key
                currentKey = c;
                pressCount = 1;
                return DecoderState.Accumulating;
            }

            // Unknown character - stay idle
            return DecoderState.Idle;
        }

        /// <summary>
        /// Handles the Accumulating state where we're building up presses of the same key.
        /// </summary>
        private static DecoderState HandleAccumulatingState(char c, ref char currentKey, ref int pressCount, StringBuilder result)
        {
            if (c == '#')
            {
                // Send - commit current and transition to idle (though we'll break after this)
                CommitCurrentKey(currentKey, pressCount, result);
                currentKey = '\0';
                pressCount = 0;
                return DecoderState.Idle;
            }

            if (c == '*')
            {
                // Backspace - first commit what we have, then backspace
                CommitCurrentKey(currentKey, pressCount, result);
                currentKey = '\0';
                pressCount = 0;

                if (result.Length > 0)
                {
                    result.Length--;
                }
                return DecoderState.Idle;
            }

            if (c == ' ')
            {
                // Space - commit current key and go idle (allows same-key repeats)
                CommitCurrentKey(currentKey, pressCount, result);
                currentKey = '\0';
                pressCount = 0;
                return DecoderState.Idle;
            }

            if (char.IsDigit(c))
            {
                // Same key? Keep accumulating
                if (c == currentKey)
                {
                    pressCount++;
                    return DecoderState.Accumulating;
                }
                else
                {
                    // Different key - commit old, start new accumulation
                    CommitCurrentKey(currentKey, pressCount, result);
                    currentKey = c;
                    pressCount = 1;
                    return DecoderState.Accumulating;
                }
            }

            // Unknown character - commit what we have and go idle
            CommitCurrentKey(currentKey, pressCount, result);
            currentKey = '\0';
            pressCount = 0;
            return DecoderState.Idle;
        }

        /// <summary>
        /// Commits the accumulated key presses to the result string.
        /// </summary>
        private static void CommitCurrentKey(char key, int presses, StringBuilder result)
        {
            if (key == '\0' || presses == 0)
            {
                return;
            }

            char outputChar = GetCharacterFromKey(key, presses);
            if (outputChar != '\0')
            {
                result.Append(outputChar);
            }
        }

        /// <summary>
        /// Maps a key and press count to the corresponding character.
        /// Handles cycling when presses exceed available characters.
        /// </summary>
        private static char GetCharacterFromKey(char key, int presses)
        {
            if (!KeypadMap.TryGetValue(key, out string? chars))
            {
                return '\0';
            }

            if (string.IsNullOrEmpty(chars))
            {
                return '\0';
            }

            // Cycle through the available chars - modulo makes it wrap around
            // Like when you'd press 2 too many times and loop back to 'A'
            int index = (presses - 1) % chars.Length;
            return char.ToUpper(chars[index]);
        }
    }
}
