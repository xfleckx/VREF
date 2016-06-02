using System;

namespace Assets.BeMoBI.Scripts.HWinterfaces
{
	public interface IStreamInfo
	{
		void StreamInfo(string name, string type);
		void StreamInfo(string name, string type, int channel_count);
		void StreamInfo(string name, string type, int channel_count, double nominal_srate);
		//void StreamInfo(string name, string type, int channel_count, double nominal_srate, channel_format_t channel_format);
		//void StreamInfo(string name, string type, int channel_count, double nominal_srate, channel_format_t channel_format, string source_id); 
		void StreamInfo(IntPtr handle);

		/// Destroy a previously created streaminfo object.
		void DestroyStreamInfo();

		// ========================
		// === Core Information ===
		// ========================
		// (these fields are assigned at construction)

		/**
        * Name of the stream.
        * This is a human-readable name. For streams offered by device modules, it refers to the type of device or product series 
        * that is generating the data of the stream. If the source is an application, the name may be a more generic or specific identifier.
        * Multiple streams with the same name can coexist, though potentially at the cost of ambiguity (for the recording app or experimenter).
        */
		string name();


		/**
        * Content type of the stream.
        * The content type is a short string such as "EEG", "Gaze" which describes the content carried by the channel (if known). 
        * If a stream contains mixed content this value need not be assigned but may instead be stored in the description of channel types.
        * To be useful to applications and automated processing systems using the recommended content types is preferred. 
        * See Table of Content Types in the documentation.
        */
		string type();

		/**
        * Number of channels of the stream.
        * A stream has at least one channel; the channel count stays constant for all samples.
        */
		int channel_count();

		/**
        * Sampling rate of the stream, according to the source (in Hz).
        * If a stream is irregularly sampled, this should be set to IRREGULAR_RATE.
        *
        * Note that no data will be lost even if this sampling rate is incorrect or if a device has temporary 
        * hiccups, since all samples will be recorded anyway (except for those dropped by the device itself). However, 
        * when the recording is imported into an application, a good importer may correct such errors more accurately 
        * if the advertised sampling rate was close to the specs of the device.
        */
		double nominal_srate();

		/**
        * Channel format of the stream.
        * All channels in a stream have the same format. However, a device might offer multiple time-synched streams 
        * each with its own format.
        */
		//channel_format_t channel_format();

		/**
        * Unique identifier of the stream's source, if available.
        * The unique source (or device) identifier is an optional piece of information that, if available, allows that
        * endpoints (such as the recording program) can re-acquire a stream automatically once it is back online.
        */
		string source_id();


		// ======================================
		// === Additional Hosting Information ===
		// ======================================
		// (these fields are implicitly assigned once bound to an outlet/inlet)

		/**
        * Protocol version used to deliver the stream.
        */
		int version();

		/**
        * Creation time stamp of the stream.
        * This is the time stamp when the stream was first created
        * (as determined via local_clock() on the providing machine).
        */
		double created_at();

		/**
        * Unique ID of the stream outlet instance (once assigned).
        * This is a unique identifier of the stream outlet, and is guaranteed to be different
        * across multiple instantiations of the same outlet (e.g., after a re-start).
        */
		string uid();
		/**
        * Session ID for the given stream.
        * The session id is an optional human-assigned identifier of the recording session.
        * While it is rarely used, it can be used to prevent concurrent recording activitites 
        * on the same sub-network (e.g., in multiple experiment areas) from seeing each other's streams 
        * (assigned via a configuration file by the experimenter, see Configuration File in the docs).
        */
		string session_id();

		/**
        * Hostname of the providing machine.
        */
		string hostname();


		// ========================
		// === Data Description ===
		// ========================

		/**
        * Extended description of the stream.
        * It is highly recommended that at least the channel labels are described here. 
        * See code examples in the documentation. Other information, such as amplifier settings, 
        * measurement units if deviating from defaults, setup information, subject information, etc., 
        * can be specified here, as well. See Meta-Data Recommendations in the docs.
        *
        * Important: if you use a stream content type for which meta-data recommendations exist, please 
        * try to lay out your meta-data in agreement with these recommendations for compatibility with other applications.
        */
		//XMLElement desc();
		/**
        * Retrieve the entire stream_info in XML format.
        * This yields an XML document (in string form) whose top-level element is <info>. The info element contains
        * one element for each field of the stream_info class, including:
        *  a) the core elements <name>, <type>, <channel_count>, <nominal_srate>, <channel_format>, <source_id>
        *  b) the misc elements <version>, <created_at>, <uid>, <session_id>, <v4address>, <v4data_port>, <v4service_port>, <v6address>, <v6data_port>, <v6service_port>
        *  c) the extended description element <desc> with user-defined sub-elements.
        */
		string as_xml();


		/**
         * Get access to the underlying handle.
         */
		IntPtr handle();


	}
}

