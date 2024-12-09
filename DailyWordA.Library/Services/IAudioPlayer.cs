namespace DailyWordA.Library.Services;

// 播放单词音频
public interface IAudioPlayer {
    public Task PlayAudioAsync(string word);
    
    public void StopAudio();
    
    public Task<bool> DownloadAudioAsync(string word);
}