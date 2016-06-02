using System;

namespace Assets.BeMoBI.Scripts.HWinterfaces
{
	public interface IStreamOutlet
	{
		/**
        * Establish a new stream outlet. This makes the stream discoverable.
        * @param info The stream information to use for creating this stream. Stays constant over the lifetime of the outlet.
        * @param chunk_size Optionally the desired chunk granularity (in samples) for transmission. If unspecified, 
        *                   each push operation yields one chunk. Inlets can override this setting.
        * @param max_buffered Optionally the maximum amount of data to buffer (in seconds if there is a nominal 
        *                     sampling rate, otherwise x100 in samples). The default is 6 minutes of data. 
        */
		void StreamOutlet(IStreamInfo info);
		void StreamOutlet(IStreamInfo info, int chunk_size); 
		void StreamOutlet(IStreamInfo info, int chunk_size, int max_buffered);

		/**
        * Destructor.
        * The stream will no longer be discoverable after destruction and all paired inlets will stop delivering data.
        */
		void DestroyStreamOutlet();


		// ========================================
		// === Pushing a sample into the outlet ===
		// ========================================

		/**
        * Push an array of values as a sample into the outlet. 
        * Each entry in the vector corresponds to one channel.
        * @param data An array of values to push (one for each channel).
        * @param timestamp Optionally the capture time of the sample, in agreement with local_clock(); if omitted, the current time is used.
        * @param pushthrough Optionally whether to push the sample through to the receivers instead of buffering it with subsequent samples.
        *                    Note that the chunk_size, if specified at outlet construction, takes precedence over the pushthrough flag.
        */
		void push_sample (float[] data);
		void push_sample (float[] data, double timestamp);
		void push_sample (float[] data, double timestamp, bool pushthrough);
		void push_sample (double[] data);
		void push_sample (double[] data, double timestamp);
		void push_sample (double[] data, double timestamp, bool pushthrough);
		void push_sample (int[] data);
		void push_sample (int[] data, double timestamp);
		void push_sample (int[] data, double timestamp, bool pushthrough);
		void push_sample (short[] data);
		void push_sample (short[] data, double timestamp);
		void push_sample (short[] data, double timestamp, bool pushthrough);
		void push_sample (char[] data);
		void push_sample (char[] data, double timestamp);
		void push_sample (char[] data, double timestamp, bool pushthrough);
		void push_sample (string[] data);
		void push_sample (string[] data, double timestamp);
		void push_sample (string[] data, double timestamp, bool pushthrough);


		// ===================================================
		// === Pushing an chunk of samples into the outlet ===
		// ===================================================

		/**
        * Push a chunk of samples into the outlet. Single timestamp provided.
        * @param data A rectangular array of values for multiple samples.
        * @param timestamp Optionally the capture time of the most recent sample, in agreement with local_clock(); if omitted, the current time is used.
        *                   The time stamps of other samples are automatically derived based on the sampling rate of the stream.
        * @param pushthrough Optionally whether to push the chunk through to the receivers instead of buffering it with subsequent samples.
        *                    Note that the chunk_size, if specified at outlet construction, takes precedence over the pushthrough flag.
        */
		void push_chunk (float[,] data);
		void push_chunk (float[,] data, double timestamp);
		void push_chunk (float[,] data, double timestamp, bool pushthrough);
		void push_chunk (double[,] data);
		void push_chunk (double[,] data, double timestamp);
		void push_chunk (double[,] data, double timestamp, bool pushthrough);
		void push_chunk (int[,] data);
		void push_chunk (int[,] data, double timestamp);
		void push_chunk (int[,] data, double timestamp, bool pushthrough);
		void push_chunk (short[,] data);
		void push_chunk (short[,] data, double timestamp);
		void push_chunk (short[,] data, double timestamp, bool pushthrough);
		void push_chunk (char[,] data);
		void push_chunk (char[,] data, double timestamp);
		void push_chunk (char[,] data, double timestamp, bool pushthrough);
		void push_chunk (string[,] data);
		void push_chunk (string[,] data, double timestamp);
		void push_chunk (string[,] data, double timestamp, bool pushthrough);
		/**
        * Push a chunk of multiplexed samples into the outlet. One timestamp per sample is provided.
        * @param data A rectangular array of values for multiple samples.
        * @param timestamps An array of timestamp values holding time stamps for each sample in the data buffer.
        * @param pushthrough Optionally whether to push the chunk through to the receivers instead of buffering it with subsequent samples.
        *                    Note that the chunk_size, if specified at outlet construction, takes precedence over the pushthrough flag.
        */
	/*	public void push_chunk(float[,] data, double[] timestamps) { dll.lsl_push_chunk_ftnp(obj, data, (uint)data.Length, timestamps, 1); }
		public void push_chunk(float[,] data, double[] timestamps, bool pushthrough) { dll.lsl_push_chunk_ftnp(obj, data, (uint)data.Length, timestamps, pushthrough ? 1 : 0); }
		public void push_chunk(double[,] data, double[] timestamps) { dll.lsl_push_chunk_dtnp(obj, data, (uint)data.Length, timestamps, 1); }
		public void push_chunk(double[,] data, double[] timestamps, bool pushthrough) { dll.lsl_push_chunk_dtnp(obj, data, (uint)data.Length, timestamps, pushthrough ? 1 : 0); }
		public void push_chunk(int[,] data, double[] timestamps) { dll.lsl_push_chunk_itnp(obj, data, (uint)data.Length, timestamps, 1); }
		public void push_chunk(int[,] data, double[] timestamps, bool pushthrough) { dll.lsl_push_chunk_itnp(obj, data, (uint)data.Length, timestamps, pushthrough ? 1 : 0); }
		public void push_chunk(short[,] data, double[] timestamps) { dll.lsl_push_chunk_stnp(obj, data, (uint)data.Length, timestamps, 1); }
		public void push_chunk(short[,] data, double[] timestamps, bool pushthrough) { dll.lsl_push_chunk_stnp(obj, data, (uint)data.Length, timestamps, pushthrough ? 1 : 0); }
		public void push_chunk(char[,] data, double[] timestamps) { dll.lsl_push_chunk_ctnp(obj, data, (uint)data.Length, timestamps, 1); }
		public void push_chunk(char[,] data, double[] timestamps, bool pushthrough) { dll.lsl_push_chunk_ctnp(obj, data, (uint)data.Length, timestamps, pushthrough ? 1 : 0); }
		public void push_chunk(string[,] data, double[] timestamps) { dll.lsl_push_chunk_strtnp(obj, data, (uint)data.Length, timestamps, 1); }
		public void push_chunk(string[,] data, double[] timestamps, bool pushthrough) { dll.lsl_push_chunk_strtnp(obj, data, (uint)data.Length, timestamps, pushthrough ? 1 : 0); }
		*/

		// ===============================
		// === Miscellaneous Functions ===
		// ===============================

		/**
        * Check whether consumers are currently registered.
        * While it does not hurt, there is technically no reason to push samples if there is no consumer.
        */
		bool have_consumers ();

		/**
        * Wait until some consumer shows up (without wasting resources).
        * @return True if the wait was successful, false if the timeout expired.
        */
		bool wait_for_consumers (double timeout);

		/**
        * Retrieve the stream info provided by this outlet.
        * This is what was used to create the stream (and also has the Additional Network Information fields assigned).
        */ 
		IStreamInfo info ();

	}


	// ===========================
	// ==== Resolve Functions ====
	// ===========================

	/**
    * Resolve all streams on the network.
    * This function returns all currently available streams from any outlet on the network.
    * The network is usually the subnet specified at the local router, but may also include 
    * a multicast group of machines (given that the network supports it), or list of hostnames.
    * These details may optionally be customized by the experimenter in a configuration file 
    * (see Network Connectivity in the LSL wiki).
    * This is the default mechanism used by the browsing programs and the recording program.
    * @param wait_time The waiting time for the operation, in seconds, to search for streams.
    *                  Warning: If this is too short (less than 0.5s) only a subset (or none) of the 
    *                           outlets that are present on the network may be returned.
    * @return An array of stream info objects (excluding their desc field), any of which can 
    *         subsequently be used to open an inlet. The full info can be retrieve from the inlet.
    */
	/*public static StreamInfo[] resolve_streams() { return resolve_streams(1.0); }
	public static StreamInfo[] resolve_streams(double wait_time)
	{
		IntPtr[] buf = new IntPtr[1024]; int num = dll.lsl_resolve_all(buf, (uint)buf.Length, wait_time);
		StreamInfo[] res = new StreamInfo[num];
		for (int k = 0; k < num; k++)
			res[k] = new StreamInfo(buf[k]);
		return res;
	}*/

	/**
    * Resolve all streams with a specific value for a given property.
    * If the goal is to resolve a specific stream, this method is preferred over resolving all streams and then selecting the desired one.
    * @param prop The stream_info property that should have a specific value (e.g., "name", "type", "source_id", or "desc/manufaturer").
    * @param value The string value that the property should have (e.g., "EEG" as the type property).
    * @param minimum Optionally return at least this number of streams.
    * @param timeout Optionally a timeout of the operation, in seconds (default: no timeout).
    *                 If the timeout expires, less than the desired number of streams (possibly none) will be returned.
    * @return An array of matching stream info objects (excluding their meta-data), any of 
    *         which can subsequently be used to open an inlet.
    */
	/*public static StreamInfo[] resolve_stream(string prop, string value) { return resolve_stream(prop, value, 1, FOREVER); }
	public static StreamInfo[] resolve_stream(string prop, string value, int minimum) { return resolve_stream(prop, value, minimum, FOREVER); }
	public static StreamInfo[] resolve_stream(string prop, string value, int minimum, double timeout)
	{
		IntPtr[] buf = new IntPtr[1024]; int num = dll.lsl_resolve_byprop(buf, (uint)buf.Length, prop, value, minimum, timeout);
		StreamInfo[] res = new StreamInfo[num];
		for (int k = 0; k < num; k++)
			res[k] = new StreamInfo(buf[k]);
		return res;
	}
	*/
	/**
    * Resolve all streams that match a given predicate.
    * Advanced query that allows to impose more conditions on the retrieved streams; the given string is an XPath 1.0 
    * predicate for the <info> node (omitting the surrounding []'s), see also http://en.wikipedia.org/w/index.php?title=XPath_1.0&oldid=474981951.
    * @param pred The predicate string, e.g. "name='BioSemi'" or "type='EEG' and starts-with(name,'BioSemi') and count(info/desc/channel)=32"
    * @param minimum Return at least this number of streams.
    * @param timeout Optionally a timeout of the operation, in seconds (default: no timeout).
    *                 If the timeout expires, less than the desired number of streams (possibly none) will be returned.
    * @return An array of matching stream info objects (excluding their meta-data), any of 
    *         which can subsequently be used to open an inlet.
    */
	/*public static StreamInfo[] resolve_stream(string pred) { return resolve_stream(pred, 1, FOREVER); }
	public static StreamInfo[] resolve_stream(string pred, int minimum) { return resolve_stream(pred, minimum, FOREVER); }
	public static StreamInfo[] resolve_stream(string pred, int minimum, double timeout)
	{
		IntPtr[] buf = new IntPtr[1024]; int num = dll.lsl_resolve_bypred(buf, (uint)buf.Length, pred, minimum, timeout);
		StreamInfo[] res = new StreamInfo[num];
		for (int k = 0; k < num; k++)
			res[k] = new StreamInfo(buf[k]);
		return res;
	}*/

}


