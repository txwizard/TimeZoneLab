/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         TimeZoneInfoExtensions

    File Name:          TimeZoneInfoExtensions.cs

    Synopsis:           This static class implements extension methods on the
                        TimeZoneInfo class that provide abbreviated renderings
                        of the time zone name strings.

    Remarks:            Extension methods are the only way to extend a
                        TimeZoneInfo object.

	License:            Copyright (C) 2021-2022, David A. Gray.
						All rights reserved.

                        Redistribution and use in source and binary forms, with
                        or without modification, are permitted provided that the
                        following conditions are met:

                        *   Redistributions of source code must retain the above
                            copyright notice, this list of conditions and the
                            following disclaimer.

                        *   Redistributions in binary form must reproduce the
                            above copyright notice, this list of conditions and
                            the following disclaimer in the documentation and/or
                            other materials provided with the distribution.

                        *   Neither the name of David A. Gray, nor the names of
                            his contributors may be used to endorse or promote
                            products derived from this software without specific
                            prior written permission.

                        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
                        CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
                        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
                        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
                        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
                        David A. Gray BE LIABLE FOR ANY DIRECT, INDIRECT,
                        INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
                        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
                        SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
                        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
                        ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
                        LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
                        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
                        IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    Created:            Sunday, 31 August 2014 - Tuesday, 01 September 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Description
    ---------- ------- ------ --------------------------------------------------
    2021/10/12 3.0     DAG    Initial implementation.

    2022/07/24 4.0     DAG    Move methods GenerateSortKey and GetTimeZoneBias
                              into this class from other classes in which they
                              were carefully tested.
    ============================================================================
*/


using System;
using System.Text;
using WizardWrx;


namespace TimeZoneLab
{
    /// <summary>
    /// Extend the sealed TimeZoneInfo class to support abbreviated time zone
    /// names.
    /// </summary>
    public static class TimeZoneInfoExtensions
    {
        /// <summary>
        /// Get the abbreviated time zone Daylight Name.
        /// </summary>
        /// <param name="ptzi">
        /// Specify the TimeZoneInfo object to process. This implicit paramteer
        /// is supplied by the runtime when the extension method is called.
        /// </param>
        /// <returns>
        /// The abbreviated name is constructed from the name specified on the
        /// DaylightName property on the TimeZoneInfo object by extracting from
        /// it the first letter of each word.
        /// </returns>
        public static string AbbreviateDaylightName ( this TimeZoneInfo ptzi )
        {
            return AbbreviateAnyTZName ( ptzi.DaylightName );
        }   // public static string AbbreviateDaylightName


        /// <summary>
        /// Get the abbreviated time zone Display Name.
        /// </summary>
        /// <param name="ptzi">
        /// Specify the TimeZoneInfo object to process. This implicit paramteer
        /// is supplied by the runtime when the extension method is called.
        /// </param>
        /// <returns>
        /// The abbreviated name is constructed from the name specified on the
        /// DisplayName property on the TimeZoneInfo object by extracting from
        /// it the first letter of each word.
        /// </returns>
        public static string AbbreviateDisplayName ( this TimeZoneInfo ptzi )
        {
            return AbbreviateAnyTZName ( ptzi.DisplayName );
        }   // public static string AbbreviateDisplayName


        /// <summary>
        /// Get the abbreviated time zone Standard Name.
        /// </summary>
        /// <param name="ptzi">
        /// Specify the TimeZoneInfo object to process. This implicit paramteer
        /// is supplied by the runtime when the extension method is called.
        /// </param>
        /// <returns>
        /// The abbreviated name is constructed from the name specified on the
        /// StandardName property on the TimeZoneInfo object by extracting from
        /// it the first letter of each word.
        /// </returns>
        public static string AbbreviatedStandardName ( this TimeZoneInfo ptzi )
        {
            return AbbreviateAnyTZName ( ptzi.StandardName );
        }   // public static string AbbreviatedStandardName


        /// <summary>
        /// Generate a usable sort key from a TimeZoneInfo object.
        /// </summary>
        /// <param name="ptzi">
        /// This argument receives a reference to the TimeZoneInfo object from
        /// which to generate a sort key.
        /// </param>
        /// <returns>
        /// <para>
        /// The return value is a string constructed as follows.
        /// </para>
        /// <list type="number">
        /// <item>
        /// A character, N for Negative and P for Positive, indicates whether
        /// the magnitude of the bias is negative, for time zones that lie to
        /// the West of Zulu, or positive, for time zones that lie to the East
        /// of Zulu.
        /// </item>
        /// <item>
        /// For time zones that lie to the West of Zulu, the absolute value of
        /// the bias is subtracted from its maximum possible value, 720 minutes,
        /// or 12 hours, and padded to 3 character positions by left-padding
        /// with zeros.
        /// </item>
        /// <item>
        /// For time zones that lie to the East of Zulu, the time zone bias is
        /// taken at face value, and padded to 3 character positions by
        /// left-padding with zeros.
        /// </item>
        /// <item>
        /// Finally, the Time Zone ID string is taken at face value, so that all
        /// time zones that have the same bias are sorted alphabetically by ID.
        /// </item>
        /// </list>
        /// <para>
        /// In the foregoing explanation, the term "Zulu" refers to the zero
        /// meridian, which runs through Greenwich, England, where the official
        /// Coordinated Universal Time (UTC) time zone is located.
        /// </para>
        /// </returns>
        public static string GenerateSortKey ( this TimeZoneInfo ptzi )
        {
            const int MAXIMUM_BIAS = 720;                                       // The maxium bias is 720 minutes, or 12 hours.

            const string DECIMAL_3_POSITIONS = @"D3";                           // This format string produces a decimal (integer) left padded with zeros, so that the bias always occupies 3 chracter poistions in the key.
            const string EAST_OF_ZULU = @"P";                                   // P stands for Positive bias, causing the time zones that lie to the East of Zulu to follow those that lie to its west.
            const string WEST_OF_ZULU = @"N";                                   // N stands for Negative bias, causing the time zones that lie to the West of Zulu to precede those that lie to its east.

            int intTimeZoneBias = ptzi.GetTimeZoneBias ( );                     // GetTimeZoneBias is promoted to an extension method.

            if ( intTimeZoneBias < MagicNumbers.ZERO )
            {   // To cause the time zones that lie to the west of Zulu to sort with the furthest west first, the actual bias is subtracted from the maximum bias of 12 hours.
                int intBiasSortOrder = MAXIMUM_BIAS - Math.Abs ( intTimeZoneBias );
                return string.Concat (
                    WEST_OF_ZULU ,                                              // Sort time zones that lie west ot Zulu first.
                    intBiasSortOrder.ToString ( DECIMAL_3_POSITIONS ) ,         // Format the time zone bias left-padded to 3 decimal digits.
                    ptzi.Id );                                                  // Take the Time Zone ID at face value.
            }   // TRUE (The time zone lies to the WEST of meridian zero.) block, if ( intTimeZoneBias < MagicNumbers.ZERO )
            else
            {   // For time zones that lie to the east of Zulu, the bias is taken at face value.
                return string.Concat (
                    EAST_OF_ZULU ,                                              // Sort time zones that lie west ot Zulu first.
                    intTimeZoneBias.ToString ( DECIMAL_3_POSITIONS ) ,          // Format the time zone bias left-padded to 3 decimal digits.
                    ptzi.Id );                                                  // Take the Time Zone ID at face value.
            }   // FALSE (The time zone lies to the EAST of meridian zero.) block, if ( intTimeZoneBias < MagicNumbers.ZERO )
        }   // private static string GenerateSortKey


        /// <summary>
        /// Given the ID of a time zone as rendered by its TimeZoneInfo object,
        /// get the StandardBias
        /// </summary>
        /// <param name="pstrTimeZoneId"></param>
        /// <returns>
        /// The return value is a signed integer that has a value less than or
        /// equal to plus or minus 720 minutes, equal to 12 hours.
        /// </returns>
        public static int GetTimeZoneBias ( this TimeZoneInfo ptzi )
        {   // These TimeSpan values are Double Precsion Floating Point values, always signed.
            return ( int ) ptzi.BaseUtcOffset.TotalMinutes;
        }   // public static int GetTimeZoneBias


        /// <summary>
        /// Construct the abbreviated time zone name.
        /// </summary>
        /// <param name="pstrTimeZoneName">
        /// This string specifies one of the three time zone name properties on
        /// the TimeZoneInfo object that was passed into the method that called
        /// it.
        /// </param>
        /// <returns>
        /// <para>
        /// The abbreviated name is constructed from the name specified on the
        /// StandardName property on the TimeZoneInfo object by extracting from
        /// it the first letter of each word.
        /// </para>
        /// <para>
        /// Two special cases exist.
        /// </para>
        /// <list type="number">
        /// <item>
        /// Regional time zones (e .g., Mexico) display the region in
        /// parentheses. The abbreviations enclose the first character of the
        /// region name in parentheses.
        /// </item>
        /// <item>
        /// Time zones that are known only by their UTC offsets have names like
        /// "UTC+11" that are abbreviated as "U+11" by taking the plus character
        /// and everything that follows it.
        /// </item>
        /// </list>
        /// </returns>
        private static string AbbreviateAnyTZName ( string pstrTimeZoneName )
        {
            const char MINUS = '-';
            const char PLUS = '+';

            int intNameLength = pstrTimeZoneName.Length;
            bool fIsFirstCharacterOfWord = true;
            bool fTakeRemainingCharacters = false;

            StringBuilder rsbAbbreviatedName = new StringBuilder ( intNameLength );

            for ( int intJ = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                      intJ < intNameLength ;
                      intJ++ )
            {
                char chrCurrent = pstrTimeZoneName [ intJ ];

                if ( fTakeRemainingCharacters )
                {
                    rsbAbbreviatedName.Append ( chrCurrent );
                }
                else if ( chrCurrent == SpecialCharacters.SPACE_CHAR )
                {
                    fIsFirstCharacterOfWord = true;
                }   // TRUE block, if ( chrCurrent == SpecialCharacters.SPACE_CHAR )
                else
                {
                    if ( fIsFirstCharacterOfWord )
                    {
                        rsbAbbreviatedName.Append ( chrCurrent );

                        if ( chrCurrent == SpecialCharacters.PARENTHESIS_LEFT )
                        {
                            //  Do nothing.
                        }
                        else if ( chrCurrent == SpecialCharacters.PARENTHESIS_RIGHT )
                        {
                            rsbAbbreviatedName.Append ( chrCurrent );
                        }
                        else
                        {
                            fIsFirstCharacterOfWord = false;
                        }
                    }   // TRUE block, if ( fIsFirstCharacterOfWord )
                    else if ( chrCurrent == SpecialCharacters.PARENTHESIS_RIGHT )
                    {
                        rsbAbbreviatedName.Append ( chrCurrent );
                    }   // FALSE block, if ( fIsFirstCharacterOfWord )
                    else if ( chrCurrent == MINUS || chrCurrent == PLUS )
                    {
                        rsbAbbreviatedName.Append ( chrCurrent );
                        fTakeRemainingCharacters = true;
                    }   // FALSE block, else if ( chrCurrent == SpecialCharacters.PARENTHESIS_RIGHT )
                }   // FALSE block, if ( chrCurrent == SpecialCharacters.SPACE_CHAR )
            }   // for ( int intJ = ArrayInfo.ARRAY_FIRST_ELEMENT ; intJ < intNameLength ; intJ++ )

            return rsbAbbreviatedName.ToString ( );
        }   // private static string AbbreviateAnyTZName
    }   // public static class TimeZoneInfoExtensions
}   // partial namespace TimeZoneLab