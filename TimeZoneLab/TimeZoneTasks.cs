/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         TimeZoneTasks

    File Name:          TimeZoneTasks.cs

    Synopsis:           This class exposes static methods for experimenting with
                        time zones.

    Remarks:            I segregated these routines to keep the globalization
                        namespaces out of the main program and to simplify
                        finding and extracting code for use in production work.

	License:            Copyright (C) 2014-2022, David A. Gray.
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

    Created:            Saturday, 30 August 2014 - Tuesday 02 September 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Synopsis
    ---------- ------- ------ -------------------------------------------------
    2014/09/02 1.0     DAG    This is the first version.

	2014/09/04 1.1     DAG    Clean up documentation and formatting of output to
                              make the project suitable for publication.

	2016/06/15 2.0     DAG    1) Embed my three-clause BSD license, and improve
                                 the internal documentation.

							  2) Break dependence upon the deprecated SharedUtl2
                                 and ApplicationHelpers2 libraries.

							  3) Add the capability to read the adjustment rules
                                 from the Windows Registry. This requires three
                                 new methods.

									1)	EmumerateTimeZoneAdjustments reads the
										adjustment records from the Registry and
										writes the details onto the console.

									2)	ShowTransitionTimeDetails formats a set
										of transition rules for display. This
                                        beats parsing the binary TZI blob in the
										Windows Registry that holds it.

									3)	ShowDelta displays the details of the
										DST Delta, which the BCL renders as a
										TimeSpan, complete with a Ticks value
										that represents the delta in Ticks.

	2021/10/10 3.0     DAG    1) Implement time zome abbreviation generation.

							  2) Upgrade to current libraries via NuGet.

	2022/07/24 4.0     DAG    1) Sort by time zone offset.
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using WizardWrx;
using WizardWrx.ConsoleAppAids3;
using WizardWrx.Core;


namespace TimeZoneLab
{
    static class TimeZoneTasks
    {
        const string TEST_FROM_TIMEZONE_ID = TimeZoneConversionTestCaseCollection.TZ_ID_MOUNTAIN;

        //  ----------------------------------------------------------------------------
        //  The first case is a date that captured from the FTP server. The other dates
        //  test edge cases around the 2014 transition dates.
        //
        //  During 2014, Daylight Saving Time is in effect from March 9 at 2 a.m. 
        //  (local time) to November 2 at 2 a.m. (local time).
        //
        //  Reference:  "Information about the Current Daylight Saving Time (DST) Rules"
        //              http://www.nist.gov/pml/div688/dst.cfm
        //  ----------------------------------------------------------------------------

        enum ReportField
        {
            Id,
            DisplayName,
            DisplayAbbreviation,   // Version 3.0
            BaseUTCoffset,
            StandardName,
            StandardAbbreviation,   // Version 3.0
            DaylightName,
            DaylightAbbreviation,   // Version 3.0
            SupportsDST
        }   // enum ReportField

        static TimeZoneInfo s_tzTestValue = TimeZoneInfo.FindSystemTimeZoneById ( TEST_FROM_TIMEZONE_ID );


        internal static int ConvertAnyTimeZoneToUTC ( string pstrOutputLabel )
        {   // This could be re-factored, but I decided that it wasn't worth the extra complexity.

            //  -------------------------------------------------------------------------------
            //  References: 1)  "Converting Times Between Time Zones"
            //                  Microsoft .NET Framework 3.5
            //                  http://msdn.microsoft.com/en-us/library/bb397769(v=vs.90).aspx
            //
            //              2)  "How to: Access the Predefined UTC and Local Time Zone Objects"
            //                  Microsoft .NET Framework 3.5
            //                  http://msdn.microsoft.com/en-us/library/bb397767(v=vs.90).aspx
            //  -------------------------------------------------------------------------------

            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_BEGIN ,     // Message template
                pstrOutputLabel ,                           // Format Item 0
                Environment.NewLine );                      // Format Item 1

            DateTime [ ] adtmTestDates;
            int rintStatus = GetTestCaseDates ( out adtmTestDates );

            if ( rintStatus > MagicNumbers.ERROR_SUCCESS )
                return rintStatus;

            int intCaseNumber = ArrayInfo.ARRAY_FIRST_ELEMENT;

            foreach ( DateTime dtmFrom in adtmTestDates )
            {
                intCaseNumber++;
                DateTime dtmTo = TimeZoneInfo.ConvertTimeToUtc (
                    dtmFrom ,
                    s_tzTestValue );
                Console.WriteLine (
                    Properties.Resources.MSG_TZ_CONVERSION ,                    // Format Template
                    new string [ ]
                    {
                        intCaseNumber.ToString ( ) ,                            // Format Item 0
                        SysDateFormatters.FormatDateForShow ( dtmFrom ) ,          // Format Item 1
                        TZHelpers.GetDisplayTimeZone (                          // Format Item 2
                            dtmFrom ,                                           // DateTime pdtmTestDate
                            TEST_FROM_TIMEZONE_ID ) ,                           // string pstrTimeZoneID
                        SysDateFormatters.FormatDateForShow ( dtmTo ) ,            // Format Item 3
                        TZHelpers.GetDisplayTimeZone (                          // Format Item 4
                            dtmTo ,                                             // DateTime pdtmTestDate
                            TZHelpers.UTC_TIMEZONE_ID ) ,                       // string pstrTimeZoneID
                        Environment.NewLine                                     // Format Item 5
                    } );
            }   // foreach ( DateTime dtmFrom in adtmTestDates )

            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_DONE ,      // Message template
                pstrOutputLabel ,                           // Format Item 0
                Environment.NewLine );                      // Format Item 1

            return MagicNumbers.ERROR_SUCCESS;
        }   // internal static int ConvertAnyTimeZoneToUTC


        internal static int ConvertAnyTimeZoneToLocalTime ( string pstrOutputLabel )
        {   // This could be re-factored, but I decided that it wasn't worth the extra complexity.
            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_BEGIN ,     					// Message template
                pstrOutputLabel ,                           					// Format Item 0
                Environment.NewLine );                      					// Format Item 1

            DateTime [ ] adtmTestDates;
            int rintStatus = GetTestCaseDates ( out adtmTestDates );

            if ( rintStatus > MagicNumbers.ERROR_SUCCESS )
                return rintStatus;

            int intCaseNumber = ArrayInfo.ARRAY_FIRST_ELEMENT;

            foreach ( DateTime dtmFrom in adtmTestDates )
            {
                intCaseNumber++;
                DateTime dtmTo = TimeZoneInfo.ConvertTime (
                    dtmFrom ,                                                   // DateTime dateTime
                    s_tzTestValue ,                                             // TimeZoneInfo sourceTimeZone
                    TimeZoneInfo.Local );                                       // TimeZoneInfo destinationTimeZone
                Console.WriteLine (
                    Properties.Resources.MSG_TZ_CONVERSION ,                    // Format Template
                    new string [ ]
                    {
                        intCaseNumber.ToString ( ) ,							// Format Item 0
                        SysDateFormatters.FormatDateForShow ( dtmFrom ) ,			// Format Item 1
                        TZHelpers.GetDisplayTimeZone (							// Format Item 2
                            dtmFrom ,											//		DateTime pdtmTestDate
                            TEST_FROM_TIMEZONE_ID ) ,							//		string pstrTimeZoneID
                        SysDateFormatters.FormatDateForShow ( dtmTo ) ,			// Format Item 3
                        TZHelpers.GetDisplayTimeZone (							// Format Item 4
                            dtmTo ,												//		DateTime pdtmTestDate
                            TimeZoneInfo.Local.Id ) ,							//		string pstrTimeZoneID
                        Environment.NewLine										// Format Item 5
                    } );
            }   // foreach ( DateTime dtmFrom in adtmTestDates )

            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_DONE ,							// Message template
                pstrOutputLabel ,                           					// Format Item 0
                Environment.NewLine );                      					// Format Item 1

            return MagicNumbers.ERROR_SUCCESS;
        }   // internal static int ConvertAnyTimeZoneToLocalTime


        internal static int ConvertBetweenAnyTwoTimeZones ( string pstrOutputLabel )
        {
            //  ---------------------------------------------------------------------------------------------
            //  References: 1)  "Converting Times Between Time Zones"
            //                  .NET Framework 3.5
            //                  http://msdn.microsoft.com/en-us/library/bb397769(v=vs.90).aspx
            //
            //              2) "TimeZoneInfo.ConvertTimeBySystemTimeZoneId Method (DateTime, String, String)"
            //                  .NET Framework 3.5
            //                  http://msdn.microsoft.com/en-us/library/bb382058(v=vs.90).aspx
            //  ---------------------------------------------------------------------------------------------

            const string ERRMSG_TEST_FAIL = @"{1}ERROR: Raw Test Date = {0} - Item SKIPPED.{1}";

            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_BEGIN ,                                             // Message template
                pstrOutputLabel ,                                                                   // Format Item 0
                Environment.NewLine );                      										// Format Item 1

            TimeZoneConversionTestCaseCollection tzcColl = new TimeZoneConversionTestCaseCollection ( );

            foreach ( TimeZoneConversionTestCase tzTestCase in tzcColl )
            {
                try
                {
                    tzTestCase.OutputDate = TimeZoneInfo.ConvertTime (
                        tzTestCase.TestDate ,                                                       // DateTime dateTime
                        tzcColl.TZTestValue ,                                                       // TimeZoneInfo sourceTimeZone
                        tzcColl.TZConverted );                                  					// TimeZoneInfo destinationTimeZone
                    Console.WriteLine (
                        Properties.Resources.MSG_TZ_CONVERSION ,                					// Format Template
                        new string [ ]
                    {
                        tzTestCase.DisplayCaseNumber ,                                  			// Format Item 0
                        tzTestCase.DisplayTestDate ,                                    			// Format Item 1
                        tzTestCase.DisplayTestDateTimeZone ,                            			// Format Item 2
                        tzTestCase.DisplayOutputDate ,                                  			// Format Item 3
                        tzTestCase.DisplayOutputDateTimeZone ,                          			// Format Item 4
                        Environment.NewLine                                    						// Format Item 5
                    } );
                }
                catch ( Exception exAll )
                {
                    ConsoleAppStateManager casm = ConsoleAppStateManager.GetTheSingleInstance ( );
                    casm.BaseStateManager.AppExceptionLogger.ReportException ( exAll );
                    Console.WriteLine (
                        ERRMSG_TEST_FAIL ,                                                          // Format template
                        tzTestCase.DisplayTestDate ,                                                // Format Item 0
                        Environment.NewLine );                      								// Format Item 0
                }
            }   // foreach ( TimeZoneConversionTestCase tzTestCase in tzcColl)

            tzcColl.CreateReport ( );
            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_DONE ,                                              // Message template
                pstrOutputLabel ,                                                                   // Format Item 0
                Environment.NewLine );                      										// Format Item 1

            return MagicNumbers.ERROR_SUCCESS;
        }   // internal static int ConvertBetweenAnyTwoTimeZones


        internal static int EnumerateTimeZones ( string pstrOutputLabel )
        {
            //  --------------------------------------------------------------------------
            //  Reference:  "How to: Enumerate Time Zones Present on a Computer"
            //              .NET Framework 3.5
            //              http://msdn.microsoft.com/en-us/library/bb397781(v=vs.90).aspx
            //  --------------------------------------------------------------------------

            const string REPORT_TEMPLATE = @"    {0} {1} {2} {3} {4} {5} {6} {7} {8}";

            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_BEGIN ,                                             // Message template
                pstrOutputLabel ,                                                                   // Format Item 0
                Environment.NewLine );                      										// Format Item 1

            ReadOnlyCollection<TimeZoneInfo> tzCollection;
            tzCollection = TimeZoneInfo.GetSystemTimeZones ( );

            SortedDictionary<string , TimeZoneInfo> dctTimeZonesByOffset = new SortedDictionary<string , TimeZoneInfo> ( );

            foreach ( TimeZoneInfo timeZone in tzCollection )
            {
                dctTimeZonesByOffset.Add (
                    timeZone.GenerateSortKey ( ) ,          // string       key
                    timeZone );                             // TimeZoneInfo value
            }   // foreach ( TimeZoneInfo timeZone in tzCollection )

            string [ ] astrTZDictKeys = new string [ tzCollection.Count ];
            dctTimeZonesByOffset.Keys.CopyTo (
                astrTZDictKeys ,
                ArrayInfo.ARRAY_FIRST_ELEMENT );
            List<string> lstTZDictKeys = new List<string> ( astrTZDictKeys );
            lstTZDictKeys.Sort ( );
            int intLongestSortKey = WizardWrx.ReportHelpers.MaxStringLength ( lstTZDictKeys );

            //  ----------------------------------------------------------------
            //  Rather than call the LengthOfLongestString library routine, this
            //  routine uses a custom routine, MaxLength, which follows the same
            //  pattern, adding an enumerator to select the desired field from a
            //  TimeZoneInfo.
            //  ----------------------------------------------------------------

            int intMaxIdLength = MaxLength (
                tzCollection ,
                ReportField.Id ,
                Properties.Resources.RPT_LBL_ID );
            int intMaxNameLength = MaxLength (
                tzCollection ,
                ReportField.DisplayName ,
                Properties.Resources.RPT_LBL_DN );
            int intMaxBaseUTCoffsetLength = MaxLength (
                tzCollection ,
                ReportField.BaseUTCoffset ,
                Properties.Resources.RPT_LBL_UO );
            int intMaxStandardNameLength = MaxLength (
                tzCollection ,
                ReportField.StandardName ,
                Properties.Resources.RPT_LBL_SN );
            int intMaxDaylightNameLength = MaxLength (
                tzCollection ,
                ReportField.DaylightName ,
                Properties.Resources.RPT_LBL_DT );
            int intMaxSupportsDSTLength = MaxLength (
                tzCollection ,
                ReportField.SupportsDST ,
                Properties.Resources.RPT_LBL_SD );

            //  ----------------------------------------------------------------
            //  Time Zone Name Abbreviations are new in version 3.0.
            //  ----------------------------------------------------------------

            int intMaxDaylightAbbrLength = MaxLength (
                tzCollection ,
                ReportField.DaylightAbbreviation ,
                Properties.Resources.RPT_LBL_DA );
            int intMaxStandardAbbrLength = MaxLength (
                tzCollection ,
                ReportField.StandardAbbreviation ,
                Properties.Resources.RPT_LBL_SA );

            Console.WriteLine (
                REPORT_TEMPLATE ,
                new string [ ] {
                    Properties.Resources.RPT_LBL_SORT_KEY.PadRight ( intLongestSortKey ) ,
                    Properties.Resources.RPT_LBL_ID.PadRight ( intMaxIdLength ) ,
                    Properties.Resources.RPT_LBL_DN.PadRight ( intMaxNameLength ) ,
                    Properties.Resources.RPT_LBL_UO.PadRight ( intMaxBaseUTCoffsetLength ) ,
                    Properties.Resources.RPT_LBL_SN.PadRight ( intMaxStandardNameLength ) ,
                    Properties.Resources.RPT_LBL_SA.PadRight ( intMaxStandardAbbrLength ) ,
                    Properties.Resources.RPT_LBL_DT.PadRight ( intMaxDaylightNameLength ) ,
                    Properties.Resources.RPT_LBL_DA.PadRight ( intMaxDaylightAbbrLength ) ,
                    Properties.Resources.RPT_LBL_SD.PadRight ( intMaxSupportsDSTLength ) } );

            //  ----------------------------------------------------------------
            //  Padding the empty string with hyphens generates a string the
            //  length of the longest value, just what is needed to create a
            //  polished report heading.
            //  ----------------------------------------------------------------

            Console.WriteLine (
                REPORT_TEMPLATE ,
                new string [ ]
                {
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intLongestSortKey ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxIdLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxNameLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxBaseUTCoffsetLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxStandardNameLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxStandardAbbrLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxDaylightNameLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxDaylightAbbrLength ,
                        SpecialCharacters.HYPHEN ) ,
                    SpecialStrings.EMPTY_STRING.PadRight (
                        intMaxSupportsDSTLength ,
                        SpecialCharacters.HYPHEN )
                } );

            foreach ( KeyValuePair<string , TimeZoneInfo> keyValuePair in dctTimeZonesByOffset )
            {
                TimeZoneInfo timeZone = keyValuePair.Value;
                Console.WriteLine (
                    REPORT_TEMPLATE ,
                    new string [ ]
                    {
                        keyValuePair.Key.ToString ( ).PadRight ( intLongestSortKey ) ,
                        timeZone.Id.PadRight ( intMaxIdLength ) ,
                        timeZone.DisplayName.PadRight ( intMaxNameLength ) ,
                        timeZone.BaseUtcOffset.ToString ( ).PadRight ( intMaxBaseUTCoffsetLength ) ,
                        timeZone.StandardName.PadRight ( intMaxStandardNameLength ) ,
                        timeZone.AbbreviatedStandardName ( ).PadRight ( intMaxStandardAbbrLength ) ,
                        timeZone.DaylightName.PadRight ( intMaxDaylightNameLength ) ,
                        timeZone.AbbreviateDaylightName ( ).PadRight ( intMaxDaylightAbbrLength ) ,
                        timeZone.SupportsDaylightSavingTime.ToString ( ).PadRight ( intMaxSupportsDSTLength ) } );
            }   // foreach ( KeyValuePair<string , TimeZoneInfo> keyValuePair in dctTimeZonesByOffset )

            Console.WriteLine (
                Properties.Resources.MSG_OUTPUT_DONE ,												// Message template
                pstrOutputLabel ,																	// Format Item 0
                Environment.NewLine );																// Format Item 1

            return MagicNumbers.ERROR_SUCCESS;
        }   // internal static int EnumerateTimeZones ( )


        /// <summary>
        /// Read test case dates from an embedded text file resource.
        /// </summary>
        /// <param name="padtmTestDates">
        /// This parameter is an output parameter that is filled with test dates
        /// on return.
        /// </param>
        /// <returns>
        /// The return value is a status code, which is expected to be zero.
        /// </returns>
        private static int GetTestCaseDates ( out DateTime [ ] padtmTestDates )
        {
            string strTestCaseFileName = Properties.Settings.Default.EdgeCaseInputFileName;

            if ( string.IsNullOrEmpty ( strTestCaseFileName ) )
            {   // The settings file is corrupted.
                padtmTestDates = null;																// The compiler insists. It doesn't know that the method is about to croak.
                return Program.ERR_TEST_CASE_FILENAME_IS_MISSING;
            }   // TRUE (unexpected outcome) block, if ( string.IsNullOrEmpty ( strTestFileName ) )
            else if ( System.IO.File.Exists ( strTestCaseFileName ) )
            {
                string [ ] astrTestCases = System.IO.File.ReadAllLines ( strTestCaseFileName );
                int intNRecords = astrTestCases.Length;

                if ( intNRecords > TimeZoneConversionTestCaseCollection.MINIMUM_RECORD_COUNT )
                {
                    padtmTestDates = new DateTime [ intNRecords - ArrayInfo.NEXT_INDEX ];

                    for ( int intThisCase = TimeZoneConversionTestCaseCollection.MINIMUM_RECORD_COUNT ;
                              intThisCase < intNRecords ;
                              intThisCase++ )
                    {
                        string [ ] astrFields = astrTestCases [ intThisCase ].Split ( SpecialCharacters.TAB_CHAR );

                        int intFieldCount = astrFields.Length;

                        if ( intFieldCount == TimeZoneConversionTestCaseCollection.EXPECTED_FIELD_COUNT )
                        {   // The record split into the expected number of fields.
                            if ( !DateTime.TryParse (
                                astrFields [ TimeZoneConversionTestCaseCollection.POS_TEST_DATE ] ,
                                out padtmTestDates [ intThisCase - ArrayInfo.ORDINAL_FROM_INDEX ] ) )
                            {   // If the operation succeeded, its work is done, because it updates the array element.
                                string strMsg = string.Format (
                                    Properties.Resources.ERRMSG_INVALID_DATE_STRING ,
                                    astrFields [ TimeZoneConversionTestCaseCollection.POS_TEST_DATE ] ,
                                    intThisCase ,
                                    strTestCaseFileName );
                                throw new Exception ( strMsg );
                            }   // if ( !DateTime.TryParse ( astrFields [ TimeZoneConversionTestCaseCollection.POS_TEST_DATE ] , out adtmTestDates [ intThisCase - Util.ORDINAL_FROM_INDEX ] ) )
                        }   // TRUE (expected outcome) block, if ( intFieldCount == EXPECTED_FIELD_COUNT )
                        else
                        {   // The record is corrupted; it contains either too few fields or too many.
                            string strMsg = string.Format (
                                TimeZoneConversionTestCaseCollection.ERRMSG_BAD_RECORD ,            // Message template
                                new object [ ]
                                {																	// Trade the tiny performance cost for prettier code.
                                    strTestCaseFileName ,											// Format Item 0
                                    intThisCase ,													// Format Item 1
                                    astrTestCases [ intThisCase ] ,									// Format Item 2
                                    intFieldCount ,													// Format Item 3
                                    TimeZoneConversionTestCaseCollection.EXPECTED_FIELD_COUNT ,		// Format Item 4
                                    Environment.NewLine												// Format Item 5
                                } );
                            throw new ApplicationException ( strMsg );
                        }   // FALSE (UNexpected outcome) block, if ( intFieldCount == EXPECTED_FIELD_COUNT )
                    }   // for ( int intThisCase = TimeZoneConversionTestCaseCollection.MINIMUM_RECORD_COUNT ; intThisCase < intNRecords ; intThisCase++ )

                    return MagicNumbers.ERROR_SUCCESS;
                }   // TRUE (expected outcome) block, if ( intNRecords > TimeZoneConversionTestCaseCollection.MINIMUM_RECORD_COUNT )
                else
                {
                    string strMsg = string.Format (
                       TimeZoneConversionTestCaseCollection.ERRMSG_BAD_FILE ,
                        strTestCaseFileName ,
                        Environment.NewLine );
                    throw new ApplicationException ( strMsg );
                }   // FALSE (UNexpected outcome) block, if ( intNRecords > TimeZoneConversionTestCaseCollection.MINIMUM_RECORD_COUNT )
            }   // TRUE (expected outcome) block, else if ( System.IO.File.Exists ( strTestFileName ) )
            else
            {   // The file of test cases got moved.
                padtmTestDates = null;																// The compiler insists. It doesn't know that the method is about to croak.
                return Program.ERR_TEST_CASE_FILE_NOT_FOUND;
            }   // FALSE (UNexpected outcome) block, else if ( System.IO.File.Exists ( strTestFileName ) )
        }   // private static int GetTestCaseDates


        /// <summary>
        /// Compute the maximum number of characters required to display the
        /// string representation of a field in a TimeZoneInfo.
        /// </summary>
        /// <param name="ptzCollection">
        /// Supply a reference to the ReadOnlyCollection<TimeZoneInfo> to
        /// analyze.
        /// </param>
        /// <param name="penmReportField">
        /// Specify a member of the ReportField enumeration to identify the
        /// field to evaluate.
        /// </param>
        /// <param name="strFieldLabel">
        /// Supply a reference to the string that you intend to use to label the
        /// column, which determines the minimum required width.
        /// </param>
        /// <returns>
        /// The return value is the length of the longest string representation
        /// of the specified field, or the length of strFieldLabel, whichever is
        /// greater.
        /// </returns>
        private static int MaxLength (
            ReadOnlyCollection<TimeZoneInfo> ptzCollection ,
            ReportField penmReportField ,
            string strFieldLabel )
        {
            //  ----------------------------------------------------------------
            //  Initializing rintMaxLength to the length of strFieldLabel.Length
            //  saves a minute amount of execution time because any value length
            //  shorter than the label skips the update step.
            //  ----------------------------------------------------------------

            int rintMaxLength = strFieldLabel.Length;
            int intNewLength = -1;

            foreach ( TimeZoneInfo tzTimeZoneInfo in ptzCollection )
            {
                switch ( penmReportField )
                {
                    case ReportField.DisplayName:
                        intNewLength = tzTimeZoneInfo.DisplayName.Length;
                        break;  // case ReportField.DisplayName

                    case ReportField.Id:
                        intNewLength = tzTimeZoneInfo.Id.Length;
                        break;

                    case ReportField.BaseUTCoffset:
                        intNewLength = tzTimeZoneInfo.BaseUtcOffset.ToString ( ).Length;
                        break;

                    case ReportField.StandardName:
                        intNewLength = tzTimeZoneInfo.StandardName.Length;
                        break;

                    case ReportField.DaylightName:
                        intNewLength = tzTimeZoneInfo.DaylightName.Length;
                        break;

                    case ReportField.SupportsDST:
                        intNewLength = tzTimeZoneInfo.SupportsDaylightSavingTime.ToString ( ).Length;
                        break;

                    case ReportField.DaylightAbbreviation:
                        intNewLength = tzTimeZoneInfo.AbbreviateDaylightName ( ).Length;
                        break;

                    case ReportField.StandardAbbreviation:
                        intNewLength = tzTimeZoneInfo.AbbreviatedStandardName ( ).Length;
                        break;

                    case ReportField.DisplayAbbreviation:
                        intNewLength = tzTimeZoneInfo.AbbreviateDisplayName ( ).Length;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException (
                            nameof ( penmReportField ) ,
                            penmReportField ,
                            Properties.Resources.MSG_UNIMPLEMENTED );
                }   // switch ( penmReportField )

                if ( intNewLength > rintMaxLength )
                {
                    rintMaxLength = intNewLength;
                }   // if ( intNewLength > rintMaxLength )
            }   // foreach ( TimeZoneInfo tzTimeZoneInfo in ptzCollection )

            return rintMaxLength;
        }   // private static int MaxLength


        /// <summary>
        /// List the details of the Daylight Saving Time adjustment rules for a
        /// specified time zone. If no time zone is specified, list the details
        /// for the local time zone.
        /// </summary>
        /// <param name="pcmdArgs">
        /// The CmdLneArgsBasic object contains collections of fully parsed
        /// switches, named arguments, and unnamed positional arguments.
        /// </param>
        /// <returns>
        /// The return value is a status code, which becomes the status code
        /// returned by the entire program.
        /// </returns>
        internal static int EmumerateTimeZoneAdjustments ( CmdLneArgsBasic pcmdArgs )
        {
            const int ARG_POS_TZ_ID = MagicNumbers.PLUS_TWO;

            int rintStatusCode = MagicNumbers.ERROR_SUCCESS;
            string strSelectedTZID = null;

            switch ( pcmdArgs.PositionalArgsInCmdLine )
            {
                case CmdLneArgsBasic.FIRST_POSITIONAL_ARG:                                          // EmumerateTimeZoneAdjustments was called to answer a specific request, but the command is incomplete.
                    rintStatusCode = Program.ERR_MISSING_TIME_ZONE_ID;
                    break;
                case CmdLneArgsBasic.NONE:                                                          // EmumerateTimeZoneAdjustments was called as part of a complete pass of all tasks.
                    strSelectedTZID = TimeZoneInfo.Local.Id;
                    break;
                default:
                    strSelectedTZID = pcmdArgs.GetArgByPosition ( ARG_POS_TZ_ID );                  // EmumerateTimeZoneAdjustments was called with at least one additional command line argument.
                    break;
            }   // switch ( pcmdArgs.PositionalArgsInCmdLine )

            Console.WriteLine (
                "{1}Enumerating time zone adjustments:{1}{1}Selected time zone = {0}{1}" ,          // Format control string
                strSelectedTZID ,                                                                   // Format Item 0 = Time Zone ID
                Environment.NewLine );                                                              // Format Item 1 = Embedded Newline
            try
            {
                TimeZoneInfo tzSelectedZone = TimeZoneInfo.FindSystemTimeZoneById ( strSelectedTZID );
                TimeZoneInfo.AdjustmentRule [ ] atzAdjRule = tzSelectedZone.GetAdjustmentRules ( );

                for ( int intRuleIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                          intRuleIndex < atzAdjRule.Length ;
                          intRuleIndex++ )
                {
                    Console.WriteLine (
                        "Adjustment Rule # {0,3}: Adjustment Rule Start Date = {1}, Adjustment Rule End Date = {2}" ,
                        ArrayInfo.OrdinalFromIndex ( intRuleIndex ) ,
                        atzAdjRule [ intRuleIndex ].DateStart ,
                        atzAdjRule [ intRuleIndex ].DateEnd );
                    Console.WriteLine (
                        "                       Daylight Transition Start = {0}" ,
                        ShowTransitionTimeDetails ( atzAdjRule [ intRuleIndex ].DaylightTransitionStart ) );
                    Console.WriteLine (
                        "                       Daylight Transition End   = {0}" ,
                        ShowTransitionTimeDetails ( atzAdjRule [ intRuleIndex ].DaylightTransitionEnd ) );
                    Console.WriteLine (
                        "                       Time Adjustment Delta     = {0}{1}" ,
                        ShowDelta ( atzAdjRule [ intRuleIndex ].DaylightDelta ) ,
                        Environment.NewLine );
                }   // for ( int intRuleIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ; intRuleIndex < atzAdjRule.Length ; intRuleIndex++ )
            }
            catch ( TimeZoneNotFoundException )
            {
                rintStatusCode = Program.ERR_INVALID_TIME_ZONE_ID;
            }
            catch ( InvalidTimeZoneException exCorruptedTZInfo )
            {
                ConsoleAppStateManager.GetTheSingleInstance ( ).BaseStateManager.AppExceptionLogger.ReportException ( exCorruptedTZInfo );
                rintStatusCode = MagicNumbers.ERROR_RUNTIME;
            }
            catch ( Exception exAllKinds )
            {
                ConsoleAppStateManager.GetTheSingleInstance ( ).BaseStateManager.AppExceptionLogger.ReportException ( exAllKinds );
                rintStatusCode = MagicNumbers.ERROR_RUNTIME;
            }

            return rintStatusCode;
        }   // EmumerateTimeZoneAdjustments


        private static string ShowDelta ( TimeSpan pdtsTimeSpan )
        {
            return string.Format (
                "{0} hours, {1} minutes, {2} seconds, {3} milliseconds, {4} ticks" ,
                new object [ ]
                {
                    pdtsTimeSpan.Hours ,										// Format Item 0 = Hours
					pdtsTimeSpan.Minutes ,										// Format Item 1 = Minutes
					pdtsTimeSpan.Seconds ,										// Format Item 2 = Seconds
					pdtsTimeSpan.Milliseconds ,									// Format Item 3 = Milliseconds
					pdtsTimeSpan.Ticks											// Format Item 4 = Ticks
				} );
        }   // ShowDelta


        /// <summary>
        /// Format the properties of a TimeZoneInfo.TransitionTime object for
        /// display.
        /// </summary>
        /// <param name="ptzTransitionTime">
        /// Specify a reference to the TimeZoneInfo.TransitionTime object to
        /// display.
        /// </param>
        /// <returns>
        /// The returned string lists every member of the TransitionTime object.
        /// </returns>
        /// <remarks>
        /// This method is essentially a custom static TransitionTime ToString
        /// method.
        /// 
        /// A TimeZoneInfo.TransitionTime object represents a DST adjustment
        /// rule that applies to a span of dates, which are reported separately
        /// as DateTime structures.
        /// 
        /// Since it is essentially a derived value, I put the DayOfWeek string
        /// in parentheses following the Day member.
        /// 
        /// The format control string takes advantage of the way string.Format
        /// matches elements of the parameter array from which it gathers its
        /// format items, which allowed me to change the display order without
        /// disturbing the array.
        /// </remarks>
        private static string ShowTransitionTimeDetails ( TimeZoneInfo.TransitionTime ptzTransitionTime )
        {
            return string.Format (
                "IsFixedDateRule = {0}, Month = {3}, Week = {5}, Day = {1} ({2}), TimeOfDay = {4}" ,
                new object [ ]
                {
                    ptzTransitionTime.IsFixedDateRule ,						// Format Item 0 = IsFixedDateRule
					ptzTransitionTime.Day ,									// Format Item 1 = Day
					ptzTransitionTime.DayOfWeek ,							// Format Item 2 = DayOfWeek
					ptzTransitionTime.Month ,								// Format Item 3 = Month
					ptzTransitionTime.TimeOfDay ,							// Format Item 4 = TimeOfDay
					ptzTransitionTime.Week									// Format Item 5 = Week
				} );
        }   // ShowTransitionTimeDetails
    }   // class TimeZoneTasks
}   // partial namespace TimeZoneLab