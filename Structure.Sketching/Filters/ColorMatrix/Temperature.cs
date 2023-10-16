/*
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

using Structure.Sketching.Filters.ColorMatrix.BaseClasses;
using Structure.Sketching.Numerics;
using System;

namespace Structure.Sketching.Filters.ColorMatrix
{
    /// <summary>
    /// Hue matrix
    /// </summary>
    /// <seealso cref="Structure.Sketching.Filters.ColorMatrix.BaseClasses.MatrixBaseClass" />
    public class Temperature : MatrixBaseClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Temperature"/> class.
        /// </summary>
        /// <param name="value">The temperature value (1000 to 40000).</param>
        public Temperature(double value)
        {
            // Temperature must fit between 1000 and 40000 degrees.
            value = Math.Clamp(value, 1000, 40000);

            // All calculations require temperature / 100, so only do the conversion once.
            value /= 100;

            // Compute each color in turn.
            int red, green, blue;

            // First: red.
            if (value <= 66)
            {
                red = 255;
            }
            else
            {
                // Note: the R-squared value for this approximation is 0.988.
                red = (int)(329.698727446 * (Math.Pow(value - 60, -0.1332047592)));
                red = Math.Clamp(red, 0, 255);
            }

            // Second: green.
            if (value <= 66)
            {
                // Note: the R-squared value for this approximation is 0.996.
                green = (int)(99.4708025861 * Math.Log(value) - 161.1195681661);
            }
            else
            {
                // Note: the R-squared value for this approximation is 0.987.
                green = (int)(288.1221695283 * (Math.Pow(value - 60, -0.0755148492)));
            }

            green = Math.Clamp(green, 0, 255);

            // Third: blue.
            if (value >= 66)
            {
                blue = 255;
            }
            else if (value <= 19)
            {
                blue = 0;
            }
            else
            {
                // Note: the R-squared value for this approximation is 0.998.
                blue = (int)(138.5177312231 * Math.Log(value - 10) - 305.0447927307);
                blue = Math.Clamp(blue, 0, 255);
            }


            _Matrix = new Matrix5x5
            {
                M11 = red / 255f,
                M12 = 0,
                M13 = 0,
                M21 = 0,
                M22 = green / 255f,
                M23 = 0,
                M31 = 0,
                M32 = 0,
                M33 = blue / 255f,
                M44 = 1,
                M55 = 1
            };
        }

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <value>The matrix.</value>
        public override Matrix5x5 Matrix => _Matrix;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value { get; }

        /// <summary>
        /// The matrix backing field
        /// </summary>
        private Matrix5x5 _Matrix = new Matrix5x5
        (
            1f, 0f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f, 0f,
            0f, 0f, 1f, 0f, 0f,
            0f, 0f, 0f, 1f, 0f,
            0f, 0f, 0f, 0f, 1f
        );

        public struct Temperatures
        {
            public const double CandleLight = 1800;
            public const double Twilight = 2500;
            public const double Moonlight = 4000;
            public const double SunAtNoon = 5500;
            public const double CloudySky = 6500;
            public const double OutdoorShade = 7000;
            public const double ClearBlueSky = 10000;
        }
    }
}