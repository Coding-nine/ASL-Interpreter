using UnityEngine;

public class ASLTranslator : MonoBehaviour
{
    public HandController hand;

    void Update()
    {
        char letter = RecognizeLetter();
        if (letter != '-')
            Debug.Log("Detected Letter: " + letter);
    }

    // ---------- Finger States ----------
    bool IndexBent() => hand.indexMCP > 70 && hand.indexPIP > 90;
    bool MiddleBent() => hand.middleMCP > 70 && hand.middlePIP > 90;
    bool RingBent() => hand.ringMCP > 70 && hand.ringPIP > 90;
    bool PinkyBent() => hand.pinkyMCP > 70 && hand.pinkyPIP > 90;

    bool IndexPIPBent() => hand.indexPIP > 90;
    bool MiddlePIPBent() => hand.middlePIP > 90;
    bool RingPIPBent() => hand.ringPIP > 90;
    bool PinkyPIPBent() => hand.pinkyPIP > 90;
    bool ThumbIPBent() => hand.thumbIP > 70;

    bool IndexStraight() => hand.indexMCP < 20 && hand.indexPIP < 20;
    bool MiddleStraight() => hand.middleMCP < 20 && hand.middlePIP < 20;
    bool RingStraight() => hand.ringMCP < 20 && hand.ringPIP < 20;
    bool PinkyStraight() => hand.pinkyMCP < 20 && hand.pinkyPIP < 20;

    bool ThumbStraight() => hand.thumbCMC < 20;
    bool ThumbBent() => hand.thumbCMC > 35 && hand.thumbIP > 20;

    bool AllFingersBent() =>
        IndexBent() && MiddleBent() && RingBent() && PinkyBent();

    bool AllFingersPIPBent() =>
        IndexPIPBent() && MiddlePIPBent() && RingPIPBent() && PinkyPIPBent();

    bool AllFingersStraight() =>
        IndexStraight() && MiddleStraight() && RingStraight() && PinkyStraight();

    // ---------- Recognition ----------
    public char RecognizeLetter()
    {
        // A
        if (AllFingersBent() && ThumbStraight())
            return 'A';

        // B
        if (AllFingersStraight() && ThumbBent())
            return 'B';

        // C
        if (!IndexBent() && !IndexStraight() &&
            !MiddleBent() && !MiddleStraight() &&
            !RingBent() && !RingStraight() &&
            !PinkyBent() && !PinkyStraight())
            return 'C';

        // D
        if (IndexStraight() && MiddleBent() && RingBent() && PinkyBent() && ThumbBent())
            return 'D';

        // E
        if (AllFingersPIPBent() && !ThumbStraight())
            return 'E';

        // F
        if (IndexBent() &&
            MiddleStraight() && RingStraight() && PinkyStraight() && ThumbBent())
            return 'F';

        // G
        if (ThumbStraight() &&
            MiddleBent() && RingBent() && PinkyBent() && hand.indexMCP > 25 && hand.indexPIP < 20)
            return 'G';

        // H
        if (IndexStraight() && MiddleStraight() &&
            RingBent() && PinkyBent())
            return 'H';

        // I
        if (PinkyStraight() &&
            IndexBent() && MiddleBent() && RingBent() && ThumbBent())
            return 'I';

        // K
        if (hand.middleMCP > 40 && IndexStraight() && ThumbStraight() &&
            RingBent() && PinkyBent())
            return 'K';

        // L
        if (IndexStraight() && ThumbStraight() &&
            MiddleBent() && RingBent() && PinkyBent())
            return 'L';

        // M
        if (AllFingersBent() && hand.thumbCMC > 40 && hand.thumbIP > 40)
            return 'M';

        // N
        if (IndexBent() && MiddleBent() && RingBent() && PinkyBent() && ThumbBent())
            return 'N';

        // O
        if (!IndexStraight() && !IndexBent() &&
            !MiddleStraight() && !MiddleBent() &&
            !RingStraight() && !RingBent() &&
            !PinkyStraight() && !PinkyBent())
            return 'O';

        // P
        if (IndexStraight() && hand.middleMCP > 80 && PinkyBent() && RingBent())
            return 'P';

        // Q
        if (hand.indexMCP > 80 && MiddleBent() && PinkyBent() && RingBent())
            return 'Q';

        // R
        if (IndexStraight() && MiddleStraight() &&
            RingBent() && PinkyBent() && ThumbBent())
            return 'R';

        // S
        if (AllFingersBent() && ThumbIPBent())
            return 'S';

        // T
        if (hand.indexPIP > 80 && hand.indexMCP > 20 && MiddleBent() && RingBent() && PinkyBent() &&
            !ThumbStraight() && !ThumbBent())
            return 'T';

        // U
        if (IndexStraight() && MiddleStraight() &&
            RingBent() && PinkyBent())
            return 'U';

        // V
        if (IndexStraight() && MiddleStraight() &&
            RingBent() && PinkyBent() && ThumbBent())
            return 'V';

        // W
        if (IndexStraight() && MiddleStraight() && RingStraight() &&
            PinkyBent() && ThumbBent())
            return 'W';

        // X
        if (IndexPIPBent() && RingBent() && MiddleBent() && PinkyBent() && ThumbBent())
            return 'X';

        // Y
        if (ThumbStraight() && PinkyStraight() &&
            IndexBent() && MiddleBent() && RingBent())
            return 'Y';

        return '-';
    }
}
