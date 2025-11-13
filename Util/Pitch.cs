using System;
using System.Collections.Generic;

namespace SharpFAI.Util;
#pragma warning disable CS1591
/// <summary>
/// 所有音调
/// All tones
/// </summary>
public enum Pitch
{
    C0, Cs0, D0, Ds0, E0, F0, Fs0, G0, Gs0, A0, As0, B0,
    C1, Cs1, D1, Ds1, E1, F1, Fs1, G1, Gs1, A1, As1, B1,
    C2, Cs2, D2, Ds2, E2, F2, Fs2, G2, Gs2, A2, As2, B2,
    C3, Cs3, D3, Ds3, E3, F3, Fs3, G3, Gs3, A3, As3, B3,
    C4, Cs4, D4, Ds4, E4, F4, Fs4, G4, Gs4, A4, As4, B4,
    C5, Cs5, D5, Ds5, E5, F5, Fs5, G5, Gs5, A5, As5, B5,
    C6, Cs6, D6, Ds6, E6, F6, Fs6, G6, Gs6, A6, As6, B6,
    C7, Cs7, D7, Ds7, E7, F7, Fs7, G7, Gs7, A7, As7, B7,
    C8, Cs8, D8, Ds8, E8, F8, Fs8, G8, Gs8, A8, As8, B8
}

/// <summary>
/// 音调助手
/// Tone helper
/// </summary>
public class PitchHelper
{
    private static readonly Dictionary<Pitch, double> PitchFrequencies = new Dictionary<Pitch, double>
    {
        {Pitch.C0, 16.352}, {Pitch.Cs0, 17.324}, {Pitch.D0, 18.354}, {Pitch.Ds0, 19.445}, {Pitch.E0, 20.601}, {Pitch.F0, 21.827}, {Pitch.Fs0, 23.124}, {Pitch.G0, 24.499}, {Pitch.Gs0, 25.956}, {Pitch.A0, 27.500}, {Pitch.As0, 29.135}, {Pitch.B0, 30.868},
        {Pitch.C1, 32.703}, {Pitch.Cs1, 34.648}, {Pitch.D1, 36.708}, {Pitch.Ds1, 38.891}, {Pitch.E1, 41.203}, {Pitch.F1, 43.654}, {Pitch.Fs1, 46.249}, {Pitch.G1, 48.999}, {Pitch.Gs1, 51.913}, {Pitch.A1, 55.000}, {Pitch.As1, 58.270}, {Pitch.B1, 61.735},
        {Pitch.C2, 65.406}, {Pitch.Cs2, 69.296}, {Pitch.D2, 73.416}, {Pitch.Ds2, 77.782}, {Pitch.E2, 82.407}, {Pitch.F2, 87.307}, {Pitch.Fs2, 92.499}, {Pitch.G2, 97.999}, {Pitch.Gs2, 103.83}, {Pitch.A2, 110.00}, {Pitch.As2, 116.54}, {Pitch.B2, 123.47},
        {Pitch.C3, 130.81}, {Pitch.Cs3, 138.59}, {Pitch.D3, 146.83}, {Pitch.Ds3, 155.56}, {Pitch.E3, 164.81}, {Pitch.F3, 174.61}, {Pitch.Fs3, 185.00}, {Pitch.G3, 196.00}, {Pitch.Gs3, 207.65}, {Pitch.A3, 220.00}, {Pitch.As3, 233.08}, {Pitch.B3, 246.94},
        {Pitch.C4, 261.63}, {Pitch.Cs4, 277.18}, {Pitch.D4, 293.66}, {Pitch.Ds4, 311.13}, {Pitch.E4, 329.63}, {Pitch.F4, 349.23}, {Pitch.Fs4, 369.99}, {Pitch.G4, 392.00}, {Pitch.Gs4, 415.30}, {Pitch.A4, 440.00}, {Pitch.As4, 466.16}, {Pitch.B4, 493.88},
        {Pitch.C5, 523.25}, {Pitch.Cs5, 554.37}, {Pitch.D5, 587.33}, {Pitch.Ds5, 622.25}, {Pitch.E5, 659.25}, {Pitch.F5, 698.46}, {Pitch.Fs5, 739.99}, {Pitch.G5, 783.99}, {Pitch.Gs5, 830.61}, {Pitch.A5, 880.00}, {Pitch.As5, 932.33}, {Pitch.B5, 987.77},
        {Pitch.C6, 1046.5}, {Pitch.Cs6, 1108.7}, {Pitch.D6, 1174.7}, {Pitch.Ds6, 1244.5}, {Pitch.E6, 1318.5}, {Pitch.F6, 1396.9}, {Pitch.Fs6, 1479.2}, {Pitch.G6, 1567.9}, {Pitch.Gs6, 1661.2}, {Pitch.A6, 1760.0}, {Pitch.As6, 1864.7}, {Pitch.B6, 1975.5},
        {Pitch.C7, 2093.0}, {Pitch.Cs7, 2217.5}, {Pitch.D7, 2349.3}, {Pitch.Ds7, 2489.0}, {Pitch.E7, 2637.0}, {Pitch.F7, 2793.8}, {Pitch.Fs7, 2959.9}, {Pitch.G7, 3135.9}, {Pitch.Gs7, 3322.4}, {Pitch.A7, 3520.0}, {Pitch.As7, 3729.3}, {Pitch.B7, 3951.1},
        {Pitch.C8, 4186.0}, {Pitch.Cs8, 4434.9}, {Pitch.D8, 4698.6}, {Pitch.Ds8, 4978.0}, {Pitch.E8, 5274.0}, {Pitch.F8, 5587.7}, {Pitch.Fs8, 5919.9}, {Pitch.G8, 6271.9}, {Pitch.Gs8, 6644.9}, {Pitch.A8, 7040.0}, {Pitch.As8, 7458.6}, {Pitch.B8, 7902.1}
    };

    private static readonly Dictionary<string, Pitch> StringToPitch = new Dictionary<string, Pitch>
    {
        {"C0", Pitch.C0}, {"C#0", Pitch.Cs0}, {"D0", Pitch.D0}, {"D#0", Pitch.Ds0}, {"E0", Pitch.E0}, {"F0", Pitch.F0}, {"F#0", Pitch.Fs0}, {"G0", Pitch.G0}, {"G#0", Pitch.Gs0}, {"A0", Pitch.A0}, {"A#0", Pitch.As0}, {"B0", Pitch.B0},
        {"C1", Pitch.C1}, {"C#1", Pitch.Cs1}, {"D1", Pitch.D1}, {"D#1", Pitch.Ds1}, {"E1", Pitch.E1}, {"F1", Pitch.F1}, {"F#1", Pitch.Fs1}, {"G1", Pitch.G1}, {"G#1", Pitch.Gs1}, {"A1", Pitch.A1}, {"A#1", Pitch.As1}, {"B1", Pitch.B1},
        {"C2", Pitch.C2}, {"C#2", Pitch.Cs2}, {"D2", Pitch.D2}, {"D#2", Pitch.Ds2}, {"E2", Pitch.E2}, {"F2", Pitch.F2}, {"F#2", Pitch.Fs2}, {"G2", Pitch.G2}, {"G#2", Pitch.Gs2}, {"A2", Pitch.A2}, {"A#2", Pitch.As2}, {"B2", Pitch.B2},
        {"C3", Pitch.C3}, {"C#3", Pitch.Cs3}, {"D3", Pitch.D3}, {"D#3", Pitch.Ds3}, {"E3", Pitch.E3}, {"F3", Pitch.F3}, {"F#3", Pitch.Fs3}, {"G3", Pitch.G3}, {"G#3", Pitch.Gs3}, {"A3", Pitch.A3}, {"A#3", Pitch.As3}, {"B3", Pitch.B3},
        {"C4", Pitch.C4}, {"C#4", Pitch.Cs4}, {"D4", Pitch.D4}, {"D#4", Pitch.Ds4}, {"E4", Pitch.E4}, {"F4", Pitch.F4}, {"F#4", Pitch.Fs4}, {"G4", Pitch.G4}, {"G#4", Pitch.Gs4}, {"A4", Pitch.A4}, {"A#4", Pitch.As4}, {"B4", Pitch.B4},
        {"C5", Pitch.C5}, {"C#5", Pitch.Cs5}, {"D5", Pitch.D5}, {"D#5", Pitch.Ds5}, {"E5", Pitch.E5}, {"F5", Pitch.F5}, {"F#5", Pitch.Fs5}, {"G5", Pitch.G5}, {"G#5", Pitch.Gs5}, {"A5", Pitch.A5}, {"A#5", Pitch.As5}, {"B5", Pitch.B5},
        {"C6", Pitch.C6}, {"C#6", Pitch.Cs6}, {"D6", Pitch.D6}, {"D#6", Pitch.Ds6}, {"E6", Pitch.E6}, {"F6", Pitch.F6}, {"F#6", Pitch.Fs6}, {"G6", Pitch.G6}, {"G#6", Pitch.Gs6}, {"A6", Pitch.A6}, {"A#6", Pitch.As6}, {"B6", Pitch.B6},
        {"C7", Pitch.C7}, {"C#7", Pitch.Cs7}, {"D7", Pitch.D7}, {"D#7", Pitch.Ds7}, {"E7", Pitch.E7}, {"F7", Pitch.F7}, {"F#7", Pitch.Fs7}, {"G7", Pitch.G7}, {"G#7", Pitch.Gs7}, {"A7", Pitch.A7}, {"A#7", Pitch.As7}, {"B7", Pitch.B7},
        {"C8", Pitch.C8}, {"C#8", Pitch.Cs8}, {"D8", Pitch.D8}, {"D#8", Pitch.Ds8}, {"E8", Pitch.E8}, {"F8", Pitch.F8}, {"F#8", Pitch.Fs8}, {"G8", Pitch.G8}, {"G#8", Pitch.Gs8}, {"A8", Pitch.A8}, {"A#8", Pitch.As8}, {"B8", Pitch.B8}
    };

    /// <summary>
    /// 获取指定音符的频率
    /// Get the frequency of the specified note
    /// </summary>
    /// <param name="pitch">音符 / Note</param>
    public static double GetFrequency(Pitch pitch)
    {
        return PitchFrequencies[pitch];
    }


    /// <summary>
    /// 将字符串表示的音符解析为Pitch枚举值
    /// Parse the string representation of a note into a Pitch enum value
    /// </summary>
    /// <param name="note">要解析的音符字符串，例如"C"、"D#"、"E♭"等 / The note string to be parsed, such as "C", "D#", "E♭", etc.</param>
    /// <returns>对应的Pitch枚举值 / The corresponding Pitch enum value</returns>
    /// <exception cref="ArgumentException">当输入的音符字符串无效时抛出 / Thrown when the input note string is invalid.</exception>
    public static Pitch ParsePitch(string note)
    {
        note = note.ToUpper();
        
        if (StringToPitch.TryGetValue(note, out Pitch pitch))
        {
            return pitch;
        }
        if (note.Contains("B"))
        {
            string unFlattenNote = note.Replace("B", "");
            if (StringToPitch.TryGetValue(unFlattenNote, out Pitch basePitch))
            {
                // 获取降半音的音符
                int baseValue = (int)basePitch;
                if (baseValue > 0)
                {
                    return (Pitch)(baseValue - 1);
                }
            }
        }
        else if (note.Contains("♭"))
        {
            string flatNote = note.Replace("♭", "B");
            return ParsePitch(flatNote);
        }
        
        throw new ArgumentException($"Invalid note: {note}");
    }

    internal static double GetGlidePitch(Pitch startNote, Pitch endNote, double percent)
    {
        double startFreq = GetFrequency(startNote);
        double endFreq = GetFrequency(endNote);
        double deltaCents = 1200 * Math.Log(endFreq / startFreq, 2);
        double currentCents = deltaCents * percent;
        return startFreq * Math.Pow(2, currentCents / 1200);
    }
}