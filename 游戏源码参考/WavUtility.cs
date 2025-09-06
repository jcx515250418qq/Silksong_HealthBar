using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class WavUtility
{
	public static void WriteWavFile(AudioClip clip, string filePath)
	{
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		using FileStream fileStream = CreateEmptyWavFile(filePath, clip.samples, clip.channels, clip.frequency);
		ConvertAndWrite(fileStream, array);
		WriteHeader(fileStream, clip);
	}

	private static FileStream CreateEmptyWavFile(string filePath, int sampleCount, int channels, int sampleRate)
	{
		FileStream fileStream = new FileStream(filePath, FileMode.Create);
		byte[] array = new byte[44];
		fileStream.Write(array, 0, array.Length);
		return fileStream;
	}

	private static void ConvertAndWrite(FileStream fileStream, float[] samples)
	{
		byte[] array = new byte[samples.Length * 2];
		int num = 32767;
		for (int i = 0; i < samples.Length; i++)
		{
			BitConverter.GetBytes((short)(samples[i] * (float)num)).CopyTo(array, i * 2);
		}
		fileStream.Write(array, 0, array.Length);
	}

	private static void WriteHeader(FileStream fileStream, AudioClip clip)
	{
		int frequency = clip.frequency;
		int channels = clip.channels;
		int samples = clip.samples;
		fileStream.Seek(0L, SeekOrigin.Begin);
		byte[] bytes = Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(bytes, 0, 4);
		byte[] bytes2 = BitConverter.GetBytes(fileStream.Length - 8);
		fileStream.Write(bytes2, 0, 4);
		byte[] bytes3 = Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(bytes3, 0, 4);
		byte[] bytes4 = Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(bytes4, 0, 4);
		byte[] bytes5 = BitConverter.GetBytes(16);
		fileStream.Write(bytes5, 0, 4);
		byte[] bytes6 = BitConverter.GetBytes((ushort)1);
		fileStream.Write(bytes6, 0, 2);
		byte[] bytes7 = BitConverter.GetBytes(channels);
		fileStream.Write(bytes7, 0, 2);
		byte[] bytes8 = BitConverter.GetBytes(frequency);
		fileStream.Write(bytes8, 0, 4);
		byte[] bytes9 = BitConverter.GetBytes(frequency * channels * 2);
		fileStream.Write(bytes9, 0, 4);
		ushort value = (ushort)(channels * 2);
		fileStream.Write(BitConverter.GetBytes(value), 0, 2);
		byte[] bytes10 = BitConverter.GetBytes((ushort)16);
		fileStream.Write(bytes10, 0, 2);
		byte[] bytes11 = Encoding.UTF8.GetBytes("data");
		fileStream.Write(bytes11, 0, 4);
		byte[] bytes12 = BitConverter.GetBytes(samples * channels * 2);
		fileStream.Write(bytes12, 0, 4);
	}
}
