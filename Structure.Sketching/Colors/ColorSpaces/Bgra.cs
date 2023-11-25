/*
Copyright 2016 James Craig
Copyright 2023 Ho Tzin Mein

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Structure.Sketching.Colors.ColorSpaces.Interfaces;
using Structure.Sketching.ExtensionMethods;
using System;
using System.Runtime.CompilerServices;

namespace Structure.Sketching.Colors.ColorSpaces;

/// <summary>
/// BGRA color space
/// </summary>
/// <seealso cref="IColorSpace"/>
/// <seealso cref="IEquatable{Bgra}"/>
public struct Bgra : IEquatable<Bgra>, IColorSpace
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Bgra"/> struct.
    /// </summary>
    /// <param name="blue">The blue.</param>
    /// <param name="green">The green.</param>
    /// <param name="red">The red.</param>
    /// <param name="alpha">The alpha.</param>
    public Bgra(byte blue, byte green, byte red, byte alpha = 255)
    {
        _r = red.Clamp(0, 255);
        _g = green.Clamp(0, 255);
        _b = blue.Clamp(0, 255);
        _a = alpha.Clamp(0, 255);
    }

    /// <summary>
    /// Gets or sets the alpha.
    /// </summary>
    /// <value>The alpha.</value>
    public byte Alpha
    {
        readonly get { return _a; }
        set { _a = value; }
    }

    /// <summary>
    /// Gets or sets the blue.
    /// </summary>
    /// <value>The blue.</value>
    public byte Blue
    {
        readonly get { return _b; }
        set { _b = value; }
    }

    /// <summary>
    /// Gets or sets the green.
    /// </summary>
    /// <value>The green.</value>
    public byte Green
    {
        readonly get { return _g; }
        set { _g = value; }
    }

    /// <summary>
    /// Gets or sets the red.
    /// </summary>
    /// <value>The red.</value>
    public byte Red
    {
        readonly get { return _r; }
        set { _r = value; }
    }

    /// <summary>
    /// alpha component
    /// </summary>
    private byte _a;

    /// <summary>
    /// blue component
    /// </summary>
    private byte _b;

    /// <summary>
    /// green component
    /// </summary>
    private byte _g;

    /// <summary>
    /// red component
    /// </summary>
    private byte _r;

    /// <summary>
    /// Performs an implicit conversion from <see cref="Color"/> to <see cref="Bgra"/>.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Bgra(Color color)
    {
        return new Bgra(color.Blue, color.Green, color.Red, color.Alpha);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Bgra"/> to <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Color(Bgra color)
    {
        return new Color(color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Bgra"/> to <see cref="int"/>.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator int(Bgra color)
    {
        return color._r << 16 | color._g << 8 | color._b << 0 | color._a << 24;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
        public static bool operator !=(Bgra left, Bgra right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
        public static bool operator ==(Bgra left, Bgra right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/>, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
        public readonly override bool Equals(object obj)
    {
        return obj is Bgra bgra && Equals(bgra);
    }

    /// <summary>
    /// Determines if the items are equal
    /// </summary>
    /// <param name="other">The other Bgra color.</param>
    /// <returns>True if they are, false otherwise</returns>
        public readonly bool Equals(Bgra other)
    {
        return other._b == _b
               && other._g == _g
               && other._r == _r
               && other._a == _a;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures
    /// like a hash table.
    /// </returns>
    public readonly override int GetHashCode()
    {
        var hash = _b.GetHashCode();
        hash = ComputeHash(hash, _g);
        hash = ComputeHash(hash, _r);
        return ComputeHash(hash, _a);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public readonly override string ToString() => $"({_b:#0.##},{_g:#0.##},{_r:#0.##},{_a:#0.##})";

    /// <summary>
    /// Computes the hash.
    /// </summary>
    /// <param name="hash">The existing hash.</param>
    /// <param name="component">The component.</param>
    /// <returns>The resulting hash</returns>
    private readonly int ComputeHash(int hash, byte component)
    {
        return ((hash << 5) + hash) ^ component.GetHashCode();
    }
}