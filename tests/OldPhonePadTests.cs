using Xunit;
using OldPhonePad.FSM;
using System;

namespace OldPhonePad.FSM.Tests
{
    /// <summary>
    /// Unit tests for the OldPhonePad decoder using Finite State Machine approach.
    /// These tests cover the examples from the challenge plus edge cases.
    /// </summary>
    public class OldPhonePadDecoderTests
    {
        #region Basic Functionality Tests

        [Fact]
        public void HandlesSimpleE()
        {
            // Just like typing 'E' on a Nokia 3310
            var result = OldPhonePadDecoder.OldPhonePad("33#");
            Assert.Equal("E", result);
        }

        [Fact]
        public void DecodesBWithBackspace()
        {
            // Type "CA" then backspace the A, leaving just B
            var result = OldPhonePadDecoder.OldPhonePad("227*#");
            Assert.Equal("B", result);
        }

        [Fact]
        public void DecodesHelloNoProblem()
        {
            // The classic HELLO - brings back memories of SMS on flip phones
            var result = OldPhonePadDecoder.OldPhonePad("4433555 555666#");
            Assert.Equal("HELLO", result);
        }

        [Fact]
        public void DecodesTuringCorrectly()
        {
            // The mystery example: "8 88777444666*664#"
            // Let's decode: 8=T, space, 88=U, 777=R, 444=I, 666=O (but then *664)
            // Backspace removes O, then 66=N, 4=G
            // Result: TURING
            var result = OldPhonePadDecoder.OldPhonePad("8 88777444666*664#");
            Assert.Equal("TURING", result);
        }

        #endregion

        #region Edge Cases - Empty and Minimal Input

        [Fact]
        public void ShouldHandleJustSend()
        {
            // Nothing typed, just hit send
            var result = OldPhonePadDecoder.OldPhonePad("#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldHandleSpaceBeforeSend()
        {
            // Space then send - should be empty
            var result = OldPhonePadDecoder.OldPhonePad(" #");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldHandleBackspaceOnEmpty()
        {
            // Try to backspace when there's nothing there
            var result = OldPhonePadDecoder.OldPhonePad("*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldHandleMultipleBackspacesOnEmpty()
        {
            // Mashing backspace like a maniac
            var result = OldPhonePadDecoder.OldPhonePad("***#");
            Assert.Equal("", result);
        }

        #endregion

        #region Single Character Tests

        [Fact]
        public void ShouldDecodeFirstLetterOfEachKey()
        {
            Assert.Equal("A", OldPhonePadDecoder.OldPhonePad("2#"));
            Assert.Equal("D", OldPhonePadDecoder.OldPhonePad("3#"));
            Assert.Equal("G", OldPhonePadDecoder.OldPhonePad("4#"));
            Assert.Equal("J", OldPhonePadDecoder.OldPhonePad("5#"));
            Assert.Equal("M", OldPhonePadDecoder.OldPhonePad("6#"));
            Assert.Equal("P", OldPhonePadDecoder.OldPhonePad("7#"));
            Assert.Equal("T", OldPhonePadDecoder.OldPhonePad("8#"));
            Assert.Equal("W", OldPhonePadDecoder.OldPhonePad("9#"));
        }

        [Fact]
        public void ShouldDecodeLastLetterOfEachKey()
        {
            Assert.Equal("C", OldPhonePadDecoder.OldPhonePad("222#"));
            Assert.Equal("F", OldPhonePadDecoder.OldPhonePad("333#"));
            Assert.Equal("I", OldPhonePadDecoder.OldPhonePad("444#"));
            Assert.Equal("L", OldPhonePadDecoder.OldPhonePad("555#"));
            Assert.Equal("O", OldPhonePadDecoder.OldPhonePad("666#"));
            Assert.Equal("S", OldPhonePadDecoder.OldPhonePad("7777#"));
            Assert.Equal("V", OldPhonePadDecoder.OldPhonePad("888#"));
            Assert.Equal("Z", OldPhonePadDecoder.OldPhonePad("9999#"));
        }

        #endregion

        #region Cycling Tests

        [Fact]
        public void ShouldCycleWhenPressingTooMany()
        {
            // Press 2 six times - should cycle back to C (2->A, 22->B, 222->C, 2222->A, 22222->B, 222222->C)
            var result = OldPhonePadDecoder.OldPhonePad("222222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void ShouldCycleOn7Key()
        {
            // Key 7 has 4 letters: P, Q, R, S
            // 77777 = 5 presses = P (cycles once, lands on P again)
            var result = OldPhonePadDecoder.OldPhonePad("77777#");
            Assert.Equal("P", result);
        }

        [Fact]
        public void ShouldCycleOn9Key()
        {
            // Key 9 has 4 letters: W, X, Y, Z
            // 99999 = 5 presses = W (cycles once)
            var result = OldPhonePadDecoder.OldPhonePad("99999#");
            Assert.Equal("W", result);
        }

        #endregion

        #region Space (Pause) Tests

        [Fact]
        public void ShouldHandleSpaceBetweenSameKeys()
        {
            // "222 2 22" should give CAB (from the problem description)
            var result = OldPhonePadDecoder.OldPhonePad("222 2 22#");
            Assert.Equal("CAB", result);
        }

        [Fact]
        public void ShouldHandleMultipleSpaces()
        {
            // Multiple spaces should work the same as one
            var result = OldPhonePadDecoder.OldPhonePad("2   2#");
            Assert.Equal("AA", result);
        }

        [Fact]
        public void ShouldHandleLeadingSpaces()
        {
            // Spaces before any input
            var result = OldPhonePadDecoder.OldPhonePad("  2#");
            Assert.Equal("A", result);
        }

        #endregion

        #region Backspace Tests

        [Fact]
        public void ShouldBackspaceLastCharacter()
        {
            // Type AB then backspace
            var result = OldPhonePadDecoder.OldPhonePad("222*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldBackspaceMiddleOfWord()
        {
            // Type CD then backspace, then add D again
            // 222=C, 3=D, backspace removes D, 3=D again
            var result = OldPhonePadDecoder.OldPhonePad("2223*3#");
            Assert.Equal("CD", result);
        }

        [Fact]
        public void ShouldHandleMultipleBackspaces()
        {
            // Type ABCD then backspace twice -> AB
            var result = OldPhonePadDecoder.OldPhonePad("222333**#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldBackspaceAndContinue()
        {
            // Type CD, backspace D, then add O
            // 222=C, 3=D, backspace removes D, 666=O
            var result = OldPhonePadDecoder.OldPhonePad("2223*666#");
            Assert.Equal("CO", result);
        }

        #endregion

        #region Special Keys Tests

        [Fact]
        public void ShouldHandleKey1Symbols()
        {
            // Key 1 has symbols: &'(
            var result = OldPhonePadDecoder.OldPhonePad("1#");
            Assert.Equal("&", result);
        }

        [Fact]
        public void ShouldHandleKey1AllSymbols()
        {
            Assert.Equal("&", OldPhonePadDecoder.OldPhonePad("1#"));
            Assert.Equal("'", OldPhonePadDecoder.OldPhonePad("11#"));
            Assert.Equal("(", OldPhonePadDecoder.OldPhonePad("111#"));
        }

        [Fact]
        public void ShouldHandleKey0AsSpace()
        {
            // Key 0 is space
            var result = OldPhonePadDecoder.OldPhonePad("220#");
            Assert.Equal("B ", result);
        }

        #endregion

        #region Complex Real-World Scenarios

        [Fact]
        public void ShouldDecodeHI()
        {
            // HI: 44=H, space, 444=I
            var result = OldPhonePadDecoder.OldPhonePad("44 444#");
            Assert.Equal("HI", result);
        }

        [Fact]
        public void ShouldDecodeYES()
        {
            // YES: 999=Y, 33=E, 7777=S
            var result = OldPhonePadDecoder.OldPhonePad("999337777#");
            Assert.Equal("YES", result);
        }

        [Fact]
        public void ShouldDecodeNO()
        {
            // NO: 66 66 -> need space between same keys
            var result = OldPhonePadDecoder.OldPhonePad("66 666#");
            Assert.Equal("NO", result);
        }

        [Fact]
        public void ShouldDecodeSOS()
        {
            // SOS: 7777=S, 666=O, 7777=S (different keys so no space needed for first->second)
            var result = OldPhonePadDecoder.OldPhonePad("7777666 7777#");
            Assert.Equal("SOS", result);
        }

        [Fact]
        public void ShouldDecodeHELLO_WORLD()
        {
            // HELLO YORLD with space character between words
            // 44=H, 33=E, 555=L, space, 555=L, 666=O, 0=space, space, 999=Y, 666=O, 777=R, 555=L, 3=D
            var result = OldPhonePadDecoder.OldPhonePad("4433555 5556660 9996667775553#");
            Assert.Equal("HELLO YORLD", result);
        }

        #endregion

        #region State Transition Tests

        [Fact]
        public void ShouldTransitionFromIdleToAccumulating()
        {
            // Simple state transition test
            var result = OldPhonePadDecoder.OldPhonePad("2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void ShouldTransitionFromAccumulatingToIdle()
        {
            // Accumulate then space (transition to idle)
            var result = OldPhonePadDecoder.OldPhonePad("222 #");
            Assert.Equal("C", result);
        }

        [Fact]
        public void ShouldHandleMultipleStateTransitions()
        {
            // Complex state machine test
            var result = OldPhonePadDecoder.OldPhonePad("2 22 222 2222#");
            Assert.Equal("ABCA", result);
        }

        [Fact]
        public void StateTransitionsWithBackspace()
        {
            // Make sure FSM handles backspace state correctly
            // Accumulating -> backspace -> idle -> accumulating
            var result = OldPhonePadDecoder.OldPhonePad("22*33#");
            Assert.Equal("E", result);
        }

        [Fact]
        public void IdleToAccumulatingMultipleTimes()
        {
            // Tests idle->accumulating transition multiple times
            var result = OldPhonePadDecoder.OldPhonePad("2 3 4 5#");
            Assert.Equal("ADGJ", result);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void ShouldThrowOnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() =>
                OldPhonePadDecoder.OldPhonePad(null!));
        }

        [Fact]
        public void ShouldThrowOnMissingSend()
        {
            Assert.Throws<ArgumentException>(() =>
                OldPhonePadDecoder.OldPhonePad("222"));
        }

        #endregion

        #region Stress Tests

        [Fact]
        public void ShouldHandleLongInput()
        {
            // A really long message - like texting an essay in 2003
            // 44=H, 33=E, 555=L, space, 555=L, 666=O, space, 999=Y, 666=O, 777=R, 555=L, 3=D
            var result = OldPhonePadDecoder.OldPhonePad("4433555 555666 9996667775553#");
            Assert.Equal("HELLOYORLD", result);
        }

        [Fact]
        public void ShouldHandleManyBackspaces()
        {
            // Type something, then backspace most of it
            var result = OldPhonePadDecoder.OldPhonePad("22233344455566677788899***************2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void ShouldHandleAlternatingKeys()
        {
            // Rapidly alternating between keys (tests FSM state transitions)
            var result = OldPhonePadDecoder.OldPhonePad("23232323#");
            Assert.Equal("ADADADAD", result);
        }

        [Fact]
        public void ShouldHandleComplexMixedInput()
        {
            // Mix of everything: digits, spaces, backspaces
            var result = OldPhonePadDecoder.OldPhonePad("44 444 555 555*666#");
            Assert.Equal("HILO", result);
        }

        [Fact]
        public void ShouldHandleRapidStateChanges()
        {
            // Tests FSM with rapid transitions between states
            // 2=A, backspace removes A, 3=D, backspace removes D, 4=G, backspace removes G, 5=J
            var result = OldPhonePadDecoder.OldPhonePad("2*3*4*5#");
            Assert.Equal("J", result);
        }

        [Fact]
        public void ShouldHandleTrailingSpaces()
        {
            // Spaces at the end before send
            var result = OldPhonePadDecoder.OldPhonePad("222   #");
            Assert.Equal("C", result);
        }

        [Fact]
        public void ShouldHandleKey0Multiple()
        {
            // Multiple presses of 0 (only has one character)
            // 2=A, 0=space, 0=space, 0=space -> but consecutive 0s just give one space
            var result = OldPhonePadDecoder.OldPhonePad("2000#");
            Assert.Equal("A ", result);
        }

        #endregion
    }
}
