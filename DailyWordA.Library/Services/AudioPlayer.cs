using DailyWordA.Library.Helpers;
using NAudio.Wave;

namespace DailyWordA.Library.Services;

public class WordAudioPlayer : IAudioPlayer {
    private WaveOutEvent _waveOut;
    private AudioFileReader _audioFileReader;
    private readonly IAlertService _alertService;

    public WordAudioPlayer(IAlertService alertService) {
        _alertService = alertService;
    }

    public const string AudioFolderName = "Audios";
   
    public static readonly string AudioFolderPath =
        PathHelper.GetLocalFolderPath(AudioFolderName);
    
    private static HttpClient _httpClient = new();
    
    public async Task PlayAudioAsync(string word) {
        string filePath = GetPathFromWord(word);
        bool flag = true;
        if (!File.Exists(filePath)) {
            flag = await DownloadAudioAsync(word);
        }

        if (flag) {
            StopAudio(); //如果有正在播放的音频，先停止
            _waveOut ??= new WaveOutEvent();
            _audioFileReader = new AudioFileReader(filePath);
            _waveOut.Init(_audioFileReader);
            _waveOut.Volume = 0.7f;
            _waveOut.Play();
        }
    }

    public void StopAudio() {
        if (_waveOut != null && _waveOut.PlaybackState == PlaybackState.Playing) {
            _waveOut.Stop();
            // _waveOut.Dispose();
        }
        
        _audioFileReader?.Dispose();
    }

    public async Task<bool> DownloadAudioAsync(string word) {
        string saveFilePath = GetPathFromWord(word);
        string url = $"https://dict.youdao.com/dictvoice?type=0&audio={word}";
        try 
        {
            var audioBytes = await _httpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(saveFilePath, audioBytes); //会自动创建新文件
        }
        catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError("有道单词服务器", e.Message));
            return false;
        }
        return true;
    }

    private string GetPathFromWord(string word)
    {
        return Path.Combine(AudioFolderPath, $"{word}.mp3");
    }
    
}