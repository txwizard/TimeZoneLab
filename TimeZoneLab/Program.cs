/*
    ============================================================================

    Module Name:        Program.cs

    Namespace Name:     TimeZoneLab

    Class Name:         Program

    Synopsis:           This command line application is a test platform for
                        experimenting with times and time zones.

    Remarks:            This class module implements the Program class, which is
                        composed exclusively of the static void Main method,
                        which is functionally equivalent to the main() routine
                        of a standard C program.

						Unless you define Unless conditional compilation symbol
						LOG_EXCEPTIONS_AS_APPEVENTS, logging to the Windows
                        Event Log is disabled, so that exception reporting is
                        confined to the standard output streams, and any user at
                        any permission level can run this program, even when it
                        crashes.

						The most likely cause of a crash is that the input file
                        of time zone edge cases is missing. The file name is in
                        the EdgeCaseInputFileName setting, which has an 
						application default, but can be set per user. 

						The default value is TimeZoneConversionEdgeCases.TXT, 
						which is expected to be in the working directory. A
						sample file is included with the distribution package.
						A report covering the edge cases is created in the file
                        named in configuration key EdgeCaseReportFileName, which
						has a default value of TimeZoneConversionEdgeCases.RPT,
						and is created in the working directory, alongside the
						input file upon which it is based.

						Thanks to a new feature of the library that reports on
						exceptions, the report is sent to both Standard Output
						and Standard Error. When the standard output string is
						redirected, the exception report appears on both output
						streams. However, when standard output remains attached
						to the console, as it is by default, only one report is
						shown. Making this happens depends on a deep knowledge
						of the Windows API encoded in a native DLL that I wrote
						in C, and is the reason that this is a 32 bit program.

	License:            Copyright (C) 2014-2016, David A. Gray.
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

                              3) Use my REG_BINARY export routines to parse the
                                 raw Registry information that supports the time
                                 zone information exposed by TimeZoneInformation
                                 and other Microsoft .NET Framework classes.

                              4) Upgrade the .NET Framework target from 3.5 to
                                 4.0 to gain access to its methods for parsing
                                 the time zone adjustment rules.

	2021/10/09 3.0     DAG    1) Implement time zome abbreviation generation.

							  2) Upgrade to current libraries via NuGet.

							  3) Implement semantic versioning and deterministic
                                 assembly generation.
    ============================================================================
*/


using System;

/*  Added by DAG */

using WizardWrx;
using WizardWrx.ConsoleAppAids3;
using WizardWrx.Core;
using WizardWrx.DLLConfigurationManager;

namespace TimeZoneLab
{
    class Program
    {
        enum OutputFormat
        {
            Verbose = 0 ,                                                       // Tell all
            Terse = 1 ,                                                         // Just the facts
            None = 2 ,                                                          // Silence!
            Quiet = 2 ,                                                         // Equivalent to None
            V = 0 ,                                                             // Equivalent to Verbose
            T = 1 ,                                                             // Equivalent to Terse
            N = 2 ,                                                             // Equivalent to None
            Q = 2                                                               // Equivalent to None
        };  //  OutputFormat

        enum Task
        {
            All ,
            AnyTimeZoneToAnyOtherTimeZone ,                                     // ConvertBetweenAnyTwoTimeZones
            EnumTimeZones ,                                                     // EnumerateTimeZones
            AnyTimeZoneToUTC ,                                                  // ConvertAnyTimeZoneToUTC
            AnyTimeZoneToLocalTime ,                                            // ConvertAnyTimeZoneToLocalTime
			EnumerateTimeZoneAdjustments										// Enumerate the array returned by GetAdjustmentRules
        }   // Task

        const int ERR_RUNTIME = 1;
        const int ERR_TASK_SPECIFIER_INVALID = 2;
        const int ERR_UNIMPLEMENTED_TASK = 3;

        public const int ERR_TEST_CASE_FILENAME_IS_MISSING = 4;
        public const int ERR_TEST_CASE_FILE_NOT_FOUND = 5;

		public const int ERR_MISSING_TIME_ZONE_ID = 6;
		public const int ERR_INVALID_TIME_ZONE_ID = 7;

        static ConsoleAppStateManager s_theApp;

        //  --------------------------------------------------------------------
        //  This argument specifies one of three supported formats for the 
        //  STDOUT display.
        //  --------------------------------------------------------------------

        const char SW_OUTPUT = 'o';

        static void Main ( string [ ] args )
		{
			//  ----------------------------------------------------------------
			//	After I created the first version of this assembly, I realized
			//	that this array is used once only, and may as well be built when
			//	it is needed, and discarded when the CmdLneArgsBasic constructor
			//	returns.
			//  ----------------------------------------------------------------

			CmdLneArgsBasic cmdArgs = new CmdLneArgsBasic (
				new char [ ] { SW_OUTPUT } ,									// This program supports one switch argument, which is specified by way of a disposable single-element array.
				CmdLneArgsBasic.ArgMatching.CaseInsensitive );					// Use the Case Insensitive parsing rules.
			cmdArgs.AllowEmptyStringAsDefault = CmdLneArgsBasic.BLANK_AS_DEFAULT_ALLOWED;	// This property cannot be set by the constructor.

			s_theApp = ConsoleAppStateManager.GetTheSingleInstance ( );

			//  ----------------------------------------------------------------
			//  Although the new state manager can correctly determine the
			//	Windows subsystem in which it is running, all output options of
			//	AppExceptionLogger are disabled by default, giving the caller
			//	full control over message disposition.
			//
			//	2016/06/15 - DAG - Adding output destination Standard Output, in
			//                     addition to Standard Error, gets exceptions
			//                     recorded in both places, taking advantage of
			//                     a new feature of the ReportException method
			//                     that eliminates output to Standard Output,
			//                     unless the Standard Output is redirected.
			//
			//	                   Along the same lines, as discussed above, the
			//					   recording of exceptions in the Windows Event
			//                     Log is disabled by default, and requires that
			//					   the assembly be built with a conditional
			//					   compilation option enabled.
			//  ----------------------------------------------------------------

			s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags =
				s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags
#if LOG_EXCEPTIONS_AS_APPEVENTS
                | ExceptionLogger.OutputOptions.EventLog
#endif	// LOG_EXCEPTIONS_AS_APPEVENTS
 | ExceptionLogger.OutputOptions.Stack
				| ExceptionLogger.OutputOptions.StandardError
				| ExceptionLogger.OutputOptions.StandardOutput;

			//  ----------------------------------------------------------------
			//	After I created the first version of this assembly, I realized
			//	that this array is used once only, and may as well be built when
			//	it is needed, and discarded when LoadErrorMessageTable returns.
			//	The line comment is the corresponding exit code, defined as a
			//	constant, with assembly scope, so that any method of any class
			//	in this assembly can return it, and the correct message is
			//	generated when the main routine detects and processes a nonzero
			//	return code.
			//  ----------------------------------------------------------------

			s_theApp.BaseStateManager.LoadErrorMessageTable (
				new string [ ]
				{
		            Properties.Resources.ERRMSG_SUCCESS ,                       // ERROR_SUCCESS
			        Properties.Resources.ERRMSG_RUNTIME ,                       // ERR_RUNTIME
				    Properties.Resources.ERRMSG_TASK_SPECIFIER_INVALID ,        // ERR_TASK_SPECIFIER_INVALID
					Properties.Resources.ERRMSG_UNIMPLEMENTED_TASK ,            // ERR_UNIMPLEMENTED_TASK
					Properties.Resources.ERRMSG_TEST_CASE_FILENAME_IS_MISSING , // ERR_TEST_CASE_FILENAME_IS_MISSING
					Properties.Resources.ERRMSG_TEST_CASE_FILE_NOT_FOUND ,      // ERR_TEST_CASE_FILE_NOT_FOUND
					Properties.Resources.ERRMSG_MISSING_TIME_ZONE_ID ,			// ERR_MISSING_TIME_ZONE_ID
					Properties.Resources.ERRMSG_INVALID_TIME_ZONE_ID ,			// ERR_INVALID_TIME_ZONE_ID
				} );

			OutputFormat enmOutputFormat = OutputFormat.None;					// enmOutputFormat needs method scope, but initialization happens inside a scope block.
			{	// Confine the scope of strDeferredMessage without extracting this little block into a separate method.
				string strDeferredMessage = null;

				//  ----------------------------------------------------------------
				//	Short of returning arrays of objects, there is no way for a
				//	method to return two or more values. Since the value that is of
				//	primary interest is the OutputFormat, while strDeferredMessage
				//	goes unused most of the time, since it is reserved for reporting
				//	exceptional circumstances that can wait until after the banner
				//	is displayed, it plays second fiddle. For a method, this means
				//	that it becomes an Out parameter. Since strings are immutable,
				//	and the object is to let SetOutputFormat create a message to be
				//	displayed later, the parameter must be an Out parameter, rather
				//	than a conventional object reference.
				//  ----------------------------------------------------------------

				enmOutputFormat = SetOutputFormat (
					cmdArgs ,
					ref strDeferredMessage );

				if ( enmOutputFormat != OutputFormat.None )
				{   // Unless output is suppressed, display the standard BOJ message.
					s_theApp.DisplayBOJMessage ( );
				}   // if ( enmOutputFormat != OutputFormat.None )

				if ( !string.IsNullOrEmpty ( strDeferredMessage ) )
				{   // SetOutputFormat saves its error message, if any, in SetOutputFormat. 
					Console.WriteLine ( strDeferredMessage );
				}   // if ( !string.IsNullOrEmpty ( s_strDeferredMessage ) )
			}	// String strDeferredMessage goes out of scope, allowing any memory that it appropriated to be reclaimed.

			//	----------------------------------------------------------------
			//	Several methods use these output labels, which are read from a
			//	custom resource. The Task variable, enmAssignedTask, earns its
			//	keep because it gets printed ahead of the switch block that is
			//	its true consumer. Had I coded the call to IdentifyTeaskToRun
			//	into the switch, the printing operation would need to be
			//	repeated, one way or another, in each case block, to achieve the
			//	intended outcome.
			//	----------------------------------------------------------------

			string [ ] astrTaskOutputLabels = WizardWrx.EmbeddedTextFile.Readers.LoadTextFileFromEntryAssembly ( Properties.Resources.TASK_LABEL_FILENAME );
			Task enmAssignedTask = IdentifyTeaskToRun ( cmdArgs );

			Console.WriteLine (
				Properties.Resources.MSG_SELECTED_TASK ,
				enmAssignedTask ,
				Environment.NewLine );

			try
			{
				switch ( enmAssignedTask )
				{
					case Task.EnumTimeZones:
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.EnumerateTimeZones ( astrTaskOutputLabels [ ( int ) Task.EnumTimeZones ] );
						break;
					case Task.AnyTimeZoneToAnyOtherTimeZone:
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.ConvertBetweenAnyTwoTimeZones ( astrTaskOutputLabels [ ( int ) Task.AnyTimeZoneToAnyOtherTimeZone ] );
						break;
                    case Task.AnyTimeZoneToUTC:
                        s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.ConvertAnyTimeZoneToUTC ( astrTaskOutputLabels [ ( int ) Task.AnyTimeZoneToUTC ] );
                        break;
					case Task.AnyTimeZoneToLocalTime:
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.ConvertAnyTimeZoneToLocalTime ( astrTaskOutputLabels [ ( int ) Task.AnyTimeZoneToLocalTime ] );
						break;
					case Task.EnumerateTimeZoneAdjustments:
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.EmumerateTimeZoneAdjustments ( cmdArgs );
						break;
					case Task.All:
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.EnumerateTimeZones ( astrTaskOutputLabels [ ( int ) Task.EnumTimeZones ] );
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.ConvertBetweenAnyTwoTimeZones ( astrTaskOutputLabels [ ( int ) Task.AnyTimeZoneToAnyOtherTimeZone ] );
                        s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.ConvertAnyTimeZoneToUTC ( astrTaskOutputLabels [ ( int ) Task.AnyTimeZoneToUTC ] );
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.ConvertAnyTimeZoneToLocalTime ( astrTaskOutputLabels [ ( int ) Task.AnyTimeZoneToLocalTime ] );
						s_theApp.BaseStateManager.AppReturnCode = TimeZoneTasks.EmumerateTimeZoneAdjustments ( cmdArgs );
						break;
					default:
						s_theApp.ErrorExit ( ERR_UNIMPLEMENTED_TASK );
						break;
				}   // switch ( enmAssignedTask )

				if ( s_theApp.BaseStateManager.AppReturnCode > MagicNumbers.ERROR_SUCCESS )
				{   // Since s_theApp.BaseStateManager.AppReturnCode is cast to int, as is the framework exit code, it must be cast to uint for s_theApp.ErrorExit.
					s_theApp.ErrorExit ( ( uint ) s_theApp.BaseStateManager.AppReturnCode );
				}   // if ( s_theApp.BaseStateManager.AppReturnCode != ERR_SUCCEEDED )
			}
			catch ( Exception exAll )
			{   // The Message string is displayed, but the complete exception goes to the event log.
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exAll );

				switch ( s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags )
				{   // If StandardError or StandardOutput is ON, the message already printed.
					case ExceptionLogger.OutputOptions.StandardError:
					case ExceptionLogger.OutputOptions.StandardOutput:
						break;
					default:
						Console.WriteLine ( exAll.Message );
						break;
				}   // switch ( s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags )

				ExitWithError (
					enmOutputFormat ,
					ERR_RUNTIME );
			}   // Providing a catch block is enough to cause the program to fall through.

			Console.WriteLine (
				Properties.Resources.MSG_TASK_DONE ,
				enmAssignedTask ,
				Environment.NewLine );

#if DEBUG
            if ( enmOutputFormat == OutputFormat.None )
            {   // Suppress all output.
                s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.Silent );
            }   // TRUE block, if ( enmOutputFormat == OutputFormat.None )
            else
            {   // Display the standard exit banner.
                if ( System.Diagnostics.Debugger.IsAttached )
                    s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.WaitForOperator );
                else
                    s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.Timed );
            }   // FALSE block, if ( enmOutputFormat == OutputFormat.None )
#else
			if ( enmOutputFormat == OutputFormat.None )
			{   // Suppress all output.
				s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.Silent );
			}   // TRUE block, if ( enmOutputFormat == OutputFormat.None )
			else
			{   // Display the standard exit banner.
				s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.ExitImmediately );
			}   // FALSE block, if ( enmOutputFormat == OutputFormat.None )
#endif
		}   // static void Main


		/// <summary>
		/// This routine gets a dedicated method because it needs a dedicated
		/// catch block, since Enum.Parse throws when it gets invalid input.
		/// </summary>
		/// <param name="cmdArgs">
		/// This is the parsed command line argument list, which offers a richer
		/// data set than the raw arguments supplied by the operating system.
		/// </param>
		/// <returns>
		/// The return value, a member of the locally defined Task enumeration,
		/// is a very efficient way to pass around information about the task or
		/// tasks to be performed.
		/// </returns>
        private static Task IdentifyTeaskToRun ( CmdLneArgsBasic cmdArgs )
        {
            const int ARGBYPOS_TASK = CmdLneArgsBasic.FIRST_POSITIONAL_ARG;
			const string ARG_OMITTED = SpecialStrings.EMPTY_STRING;
            const bool ARG_PARSE_IGNORE_CASE = true;

            const string ERRMSG_INVALID_TASK_ID = @"The specified task, {0}, is invalid.";

            //  ----------------------------------------------------------------
            //  Since the string appears in error messages, it's cheaper to 
            //  stash a copy here.
            //  ----------------------------------------------------------------

            string strTaskArg = cmdArgs.GetArgByPosition (
                ARGBYPOS_TASK ,
                ARG_OMITTED );

            //  ------------------------------------------------------------
            //  The catch blocks that follow end with a return that is
            //  unreachable. The compiler doesn't know that, because it
            //  cannot possibly know that ErrorExit shuts down the program.
            //
            //  Since the method signature tells the compiler to expect this
            //  method to return a Task, it complains loudly, to the effect
            //  that not all paths return a value, which the C# compiler 
            //  treats as a fatal compilation error. In contrast, the Visual
            //  C++ compiler treats this condition as a warning.
            //  ------------------------------------------------------------

            if ( string.IsNullOrEmpty ( strTaskArg ) )
            {   // The terse message generated by ErrorExit is plenty.
                return Task.All;
            }   // if ( string.IsNullOrEmpty ( strTaskArg ) )

            try
            {
                return ( Task ) Enum.Parse (
                    typeof ( Task ) ,
                    strTaskArg ,
                    ARG_PARSE_IGNORE_CASE );
            }   // We're done. The remaining code is composed of catch blocks.
            catch ( ArgumentNullException errArgIsNull )
            {
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( errArgIsNull );
                s_theApp.ErrorExit ( ERR_RUNTIME );
                return Task.EnumTimeZones;
            }   // catch ( ArgumentNullException errArgIsNull )
            catch ( ArgumentException errArgIsInvalid )
            {
                if ( typeof ( Task ) == typeof ( Enum ) )
                {   // It isn't worth the work to evaluate the white space case.
                    Console.WriteLine (
                        ERRMSG_INVALID_TASK_ID ,
                        strTaskArg );
                    s_theApp.ErrorExit ( ERR_TASK_SPECIFIER_INVALID );
                    return Task.EnumTimeZones;
                }   // TRUE (expected outcome) block, if ( typeof ( Task ) == typeof ( Enum ) )
                else
                {   // The enumType, typeof ( Task ), is not an Enum.
					s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( errArgIsInvalid );
                    s_theApp.ErrorExit ( ERR_RUNTIME );
                    return Task.EnumTimeZones;
                }   // FALSE (UNexpected outcome) block, if ( typeof ( Task ) == typeof ( Enum ) )
            }   // catch ( ArgumentException errArgIsInvalid )
            catch ( OverflowException errOverflow )
            {
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( errOverflow );
                s_theApp.ErrorExit ( ERR_RUNTIME );
                return Task.EnumTimeZones;
            }   // catch ( OverflowException errOverflow )
            catch ( Exception errMisc )
            {   // Some totally unexpected error happened.
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( errMisc );
                s_theApp.ErrorExit ( ERR_RUNTIME );
                return Task.EnumTimeZones;
            }   // catch ( Exception errMisc )
        }   // private static Task IdentifyTeaskToRun


		/// <summary>
		/// Through its OutputFormat enumeration, this method affords a good bit
		/// of control over how error exits are handled.
		/// </summary>
		/// <param name="penmOutputFormat">
		/// The locally defined OutputFormat enumeration defines options for
		/// handling program exits, including such things as the verbosity of
		/// the output messages, and whether the program halts for a specified
		/// number of seconds, or waits forever, to allow a carbon unit (a
		/// human, in case you haven't seen Star Trek: The Movie), to see and
		/// act upon the output.
		/// </param>
		/// <param name="puintStatusCode">
		/// Thinking that it really should be unsigned, I cast the return code
		/// to Unsigned Integer. If I had it to do over, I would cast it to Int.
		/// </param>
		/// <remarks>
		/// Though the marking of void implies that this method returns, with a
		/// meaningless return value, it is designed to never return to its
		/// caller. Instead, control returns to the operating system, which is
		/// informed of the status code specified in puintStatusCode.
		/// </remarks>
        private static void ExitWithError (
            OutputFormat penmOutputFormat ,
            uint puintStatusCode )
        {
            if ( penmOutputFormat == OutputFormat.Quiet )
                s_theApp.NormalExit (
                    puintStatusCode ,
                    ConsoleAppStateManager.NormalExitAction.Silent );
            else
                if ( System.Diagnostics.Debugger.IsAttached)
                    s_theApp.NormalExit (
                        puintStatusCode ,
                        ConsoleAppStateManager.NormalExitAction.WaitForOperator );
                else
                    s_theApp.NormalExit (
                        puintStatusCode ,
                        ConsoleAppStateManager.NormalExitAction.ExitImmediately );
        }   // private static void ExitWithError


		/// <summary>
		/// While some command line arguments can be processed inline, since 
		/// this method returns an enumerated type, and the enumeration parser
		/// throws when its input is invalid, it made more sense to put it into
		/// a dedicated method, with its own self contained try/catch block.
		/// </summary>
		/// <param name="pcmdArgs">
		/// A CmdLneArgsBasic object contains a fully parsed argument list,
		/// arranged into groups comprised of valid switches, valid named
		/// arguments, and nameless positional arguments. A switch argument is 
		/// defined by a single character that serves as its name, preceded by a
		/// hyphen or forward slash, and followed by an optional modifier that
		/// acts as a value. A named argument is identified by a string of
		/// characters, followed by an equals sign and a value.
		/// 
		/// An unmodified switch can convey a Boolean state by its mere presence
		/// in the command line.
		/// 
		/// Any argument that is neither a switch, nor a named argument, is a
		/// positional argument.
		/// </param>
		/// <param name="pstrDeferredMessage">
		/// The pstrDeferredMessage string is set aside for reporting a benign
		/// error, such as an invalid command line argument, that can wait until
		/// after the banner has been displayed to be reported. This mechanism
		/// permits a Quiet (None) option to be implemented that completely
		/// suppresses output, even the banner. This feature covers use cases
		/// where extraneous output would either be confusing, or would clutter
		/// an output file that is being fed into another program for further
		/// processing.
		/// </param>
		/// <returns>
		/// The locally defined OutputFormat enumeration specifies levels of
		/// detail to display in the output, ranging from None to Verbose. To
		/// fully implement the Quiet mode, command line parsing and evaluation
		/// of this option must be the very first things that happen, before the
		/// banner is displayed, because the Quite option requires the banner to
		/// be suppressed.
		/// </returns>
        private static OutputFormat SetOutputFormat (
            CmdLneArgsBasic pcmdArgs ,
            ref string pstrDeferredMessage )
        {
            //  ----------------------------------------------------------------
            //  An invalid input value elicits a message similar to the following.
            //
            //      Requested value 'Foolish' was not found.
            //
            //  The simplest way to report an invalid value is by extracting it
            //  from the Message property of the ArgumentException thrown by the
            //  Enum.Parse method.
            //
            //  I happen to have a library routine, ExtractBoundedSubstrings,
            //  which became part of a sealed class, WizardWrx.StringTricks,
            //  exported by class library WizardWrx.SharedUtl2.dll version 2.62,
            //  which came into being exactly two years ago, 2011/11/23.
            //  ----------------------------------------------------------------

            const bool IGNORE_CASE = true;
            const int NONE = 0;

            OutputFormat renmOutputFormat = OutputFormat.Verbose;

			//  ----------------------------------------------------------------
			//  Enum.Parse needs a try/catch block, because an invalid SW_OUTPUT
			//  value raises an exception that can be gracefully handled without
			//  killing the program.
			//  ----------------------------------------------------------------

			try
			{
				if ( pcmdArgs.ValidSwitchesInCmdLine > NONE )
				{
					renmOutputFormat = ( OutputFormat ) Enum.Parse (
						typeof ( OutputFormat ) ,
						pcmdArgs.GetSwitchByName (
							SW_OUTPUT ,
							OutputFormat.Verbose.ToString ( ) ) ,
						IGNORE_CASE );
				}   // if ( pcmdArgs.ValidSwitchesInCmdLine > NONE )
			}
			catch ( ArgumentException exArg )
			{   // Display of the message is deferred until the BOJ message is printed.
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exArg );

				pstrDeferredMessage = string.Format (
					Properties.Resources.ERRMSG_INVALID_OUTPUT_FORMAT ,
					exArg.Message.ExtractBoundedSubstrings (
						SpecialCharacters.SINGLE_QUOTE ) ,
					renmOutputFormat ,
					Environment.NewLine );
			}

            return renmOutputFormat;
        }   // private static OutputFormat SetOutputFormat
    }   // class Program
}   // partial namespace TimeZoneLab