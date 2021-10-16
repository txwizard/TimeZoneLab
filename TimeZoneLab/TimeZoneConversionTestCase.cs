/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         TimeZoneConversionTestCase

    File Name:          TimeZoneConversionTestCase.cs

    Synopsis:           This class exposes a test case, and provides spaces to
                        record the answers, so that the the complete set can be
                        formatted into a fixed length ASCII report like the one
                        that I made of the time zone enumeration.

    Remarks:            This is a unit test for the TZHelpers class.

	License:            Copyright (C) 2014-2021, David A. Gray.
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
    2014/09/02 1.0     DAG    Initial implementation.

	2016/06/14 2.0     DAG    1) Embed my three-clause BSD license, and improve
                                 the internal documentation.

							  2) Break dependence upon the deprecated SharedUtl2
                                 and ApplicationHelpers2 libraries.

							  3) Eliminate the UTIL class, since all it ever did
                                 was give local scope to constants defined in
                                 WizardWrx.DLLServices2.dll, WHICH IS ALREADY
                                 bound into the project for more substantial
								 reasons.

                              4) Replace the generic IComparable interface with
                                 a more robust explicit interface.

	2021/10/10 3.0     DAG    1) Implement time zome abbreviation generation.

							  2) Upgrade to current libraries via NuGet.
    ============================================================================
*/

using System;

using WizardWrx;


namespace TimeZoneLab
{
	class TimeZoneConversionTestCase : IComparable<TimeZoneConversionTestCase>
    {
        #region Instance Storage
        uint _uintCaseNumber;
        DateTime _dtmTestDate = DateTime.MinValue;
        string _strTestDateTimeZoneID = null;
        string _strComment = null;
        DateTime _dtmOutputDate = DateTime.MinValue;
        string _strOutputDateTimeZoneID = null;
        #endregion  // Instance Storage


        #region Constructors
        public TimeZoneConversionTestCase ( )
        { } // public TimeZoneConversionTestCase constructor (1 of 2)

        public TimeZoneConversionTestCase (
            uint puintCaseNumber ,
            string pstrTestDate ,
            string pstrComment ,
            string pstrTZIDIn ,
            string pstrTZIdOut)
        {
            const string ARGNAME_COMMENT = @"pstrComment";
            const string ARGNAME_TZ_ID_IN = @"pstrTZIDIn";
            const string ARGNEME_TZ_ID_OUT=@"pstrTZIdOut";
            const string ARGNAME_TEST_DATE = @"pstrTestDate";

            const string ERRMSG_TRYPARSE = @"DateTime.TryParse cannot parse this value.";

            if ( DateTime.TryParse ( pstrTestDate , out _dtmTestDate ) )
            {
                if ( string.IsNullOrEmpty ( pstrComment ) )
                {
                    throw new ArgumentNullException ( ARGNAME_COMMENT );
                }
                else if ( string.IsNullOrEmpty ( pstrTZIDIn ) )
                {
                    throw new ArgumentNullException ( ARGNAME_TZ_ID_IN );
                }
                else if ( string.IsNullOrEmpty ( pstrTZIdOut ) )
                {
                    throw new ArgumentNullException ( ARGNEME_TZ_ID_OUT );
                }
                else
                {
                    _strComment = pstrComment;
                    _uintCaseNumber = puintCaseNumber;
                    _strTestDateTimeZoneID = pstrTZIDIn;
                    _strOutputDateTimeZoneID = pstrTZIdOut;
                }   // FALSE (desired outcome!) block, if ( string.IsNullOrEmpty ( pstrComment ) ) ...
            }   // TRUE (expected outcome) block, if ( DateTime.TryParse ( pstrTestDate , out _dtmTestDate ) )
            else
            {
                throw new ArgumentOutOfRangeException (
                    ARGNAME_TEST_DATE ,
                    pstrTestDate ,
                    ERRMSG_TRYPARSE );
            }   // expected outcome) block, if ( DateTime.TryParse ( pstrTestDate , out _dtmTestDate ) )
        }   // public TimeZoneConversionTestCase constructor (2 of 2)
        #endregion  // Constructors


        #region Properties
        public uint CaseNumber
        {
            get { return _uintCaseNumber; }
        }   // public uint CaseNumber (READ ONLY and immutable)


        public string DisplayCaseNumber
        {
            get { return _uintCaseNumber.ToString ( ); }
        }   // public string DisplayCaseNumber (READ ONLY and immutable)


        public DateTime TestDate
        {
            get { return _dtmTestDate; }
        }   // public DateTime TestDate (READ ONLY and immutable)


        public string TestDateTimeZoneID
        {
            get { return _strTestDateTimeZoneID; }
        }   // public string TestDateTimeZoneID (READ ONLY and immutable)


        public string OutputDateTimeZoneID
        {
            get 
            {
                return _strOutputDateTimeZoneID;
            }
        }   // public string OutputDateTimeZoneID (READ ONLY and immutable)


        public string Comment
        {
            get { return _strComment; }
        }   // public string Comment (READ ONLY and immutable)


        public string DisplayTestDate
        {
            get
            {
                return SysDateFormatters.FormatDateForShow ( _dtmTestDate );
            }   // public string DisplayTestDate getter
        }   // public string DisplayTestDate (READ ONLY)


        public string DisplayTestDateTimeZone
        {
            get
            {
                return TZHelpers.GetDisplayTimeZone (
                    _dtmTestDate ,
                    _strTestDateTimeZoneID );
            }   // // public string DisplayTestDateTimeZone getter
        }   // public string DisplayTestDateTimeZone (READ ONLY)


        public DateTime OutputDate
        {
            get { return _dtmOutputDate; }
            set { _dtmOutputDate = value; }
        }   // public DateTime OutputDate (Read/Write)


        public string DisplayOutputDate
        {
            get
            {
                return SysDateFormatters.FormatDateForShow ( _dtmOutputDate );
            }   // public string DisplayOutputDate getter
        }   // public string DisplayOutputDate (READ ONLY)


        public string DisplayOutputDateTimeZone
        {
            get
            {
                return TZHelpers.GetDisplayTimeZone (
                    _dtmOutputDate ,
                    _strOutputDateTimeZoneID );
            }   // // public string DisplayOutputDateTimeZone getter
        }   // public string DisplayOutputDateTimeZone (READ ONLY)


        public string [ ] ColumnValues
        {
            get
            {
                return new string [ ]
                {
                    DisplayCaseNumber ,
                    Comment ,
                    DisplayTestDate ,
                    DisplayTestDateTimeZone ,
                    DisplayOutputDate ,
                    DisplayOutputDateTimeZone
                };
            }   // public string [ ] ColumnValues getter
        }   // public string [ ] ColumnValues (READ ONLY)
        #endregion  // Properties


		#region IComparable<TimeZoneConversionTestCase> Members
		int IComparable<TimeZoneConversionTestCase>.CompareTo ( TimeZoneConversionTestCase other )
		{
			return _uintCaseNumber.CompareTo ( other._uintCaseNumber );
		}	// CompareTo
		#endregion	// IComparable<TimeZoneConversionTestCase> Members
	}   // class TimeZoneConversionTestCase
}   // partial namespace TimeZoneLab