/*
Copyright 2016 James Craig

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

namespace Structure.Sketching.Colors.ColorSpaces
{
    /// <summary>
    /// CIELAB
    /// </summary>
    /// <seealso cref="IColorSpace"/>
    /// <seealso cref="IEquatable{CIELab}"/>
    public struct CIELab : IEquatable<CIELab>, IColorSpace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CIELab"/> struct.
        /// </summary>
        /// <param name="lightness">The lightness.</param>
        /// <param name="aComponent">The a component.</param>
        /// <param name="bComponent">The b component.</param>
        public CIELab(double lightness, double aComponent, double bComponent)
        {
            L = lightness.Clamp(0, 100);
            A = aComponent.Clamp(-100, 100);
            B = bComponent.Clamp(-100, 100);
        }

        /// <summary>
        /// Gets or sets a component.
        /// </summary>
        /// <value>a component.</value>
        public double A;

        /// <summary>
        /// Gets or sets the b component.
        /// </summary>
        /// <value>The b component.</value>
        public double B;

        /// <summary>
        /// Gets or sets the lightness.
        /// </summary>
        /// <value>The lightness.</value>
        public double L;

        /// <summary>
        /// The epsilon
        /// </summary>
        private const float EPSILON = 0.001f;

        /// <summary>
        /// Intent is 24389/27
        /// </summary>
        private const double Kappa = 903.3;

        /// <summary>
        /// Intent is 216/24389
        /// </summary>
        private const double XYZEpsilon = 0.008856;

        /// <summary>
        /// Performs an implicit conversion from <see cref="Color"/> to <see cref="CIELab"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator CIELab(Color color)
        {
            return (XYZ)color;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="XYZ"/> to <see cref="CIELab"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator CIELab(XYZ color)
        {
            var white = XYZ.WhiteReference;
            var x = PivotXyz(color.X / white.X);
            var y = PivotXyz(color.Y / white.Y);
            var z = PivotXyz(color.Z / white.Z);

            return new CIELab(
                (float)Math.Max(0, 116 * y - 16),
                (float)(500 * (x - y)),
                (float)(200 * (y - z))
            );
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CIELab"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color(CIELab color)
        {
            return (XYZ)color;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CIELab"/> to <see cref="XYZ"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator XYZ(CIELab color)
        {
            var y = (color.L + 16d) / 116d;
            var x = (color.A / 500d) + y;
            var z = y - (color.B / 200d);
            var white = XYZ.WhiteReference;

            var x3 = x * x * x;
            var z3 = z * z * z;

            return new XYZ(white.X * (x3 > XYZEpsilon ? x3 : (x - 16.0 / 116.0) / 7.787),
                    white.Y * (color.L > (Kappa * XYZEpsilon) ? Math.Pow((color.L + 16.0) / 116.0, 3) : color.L / Kappa),
                    white.Z * (z3 > XYZEpsilon ? z3 : (z - 16.0 / 116.0) / 7.787));
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CIELab left, CIELab right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CIELab left, CIELab right)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is CIELab && Equals((CIELab)obj);
        }

        /// <summary>
        /// Determines if the items are equal
        /// </summary>
        /// <param name="other">The other CIELab color.</param>
        /// <returns>True if they are, false otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(CIELab other)
        {
            return Math.Abs(other.L - L) < EPSILON
                && Math.Abs(other.A - A) < EPSILON
                && Math.Abs(other.B - B) < EPSILON;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = L.GetHashCode();
            hash = ComputeHash(hash, A);
            return ComputeHash(hash, B);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString() => $"({L:#0.##},{A:#0.##},{B:#0.##})";

        /// <summary>
        /// Pivots to xyz.
        /// </summary>
        /// <param name="n">The value.</param>
        /// <returns>The resulting value</returns>
        private static double PivotXyz(double n)
        {
            return n > XYZEpsilon ? n.CubicRoot() : (Kappa * n + 16) / 116;
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="hash">The existing hash.</param>
        /// <param name="component">The component.</param>
        /// <returns>The resulting hash</returns>
        private int ComputeHash(int hash, double component)
        {
            return ((hash << 5) + hash) ^ component.GetHashCode();
        }
    }
}