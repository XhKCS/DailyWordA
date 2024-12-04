namespace DailyWordA.Library.Services;

public interface IAudioPlayer {
    public Task PlayAudioAsync(string word);
    
    public void StopAudio();
    
    public Task<bool> DownloadAudioAsync(string word);
}