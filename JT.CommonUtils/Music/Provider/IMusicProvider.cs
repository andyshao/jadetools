using System.Collections.Generic;
using JT.CommonUtils.Music.Domain;

namespace JT.CommonUtils.Music.Provider
{
    public interface IMusicProvider
    {
        string Name { get; }

        string getDownloadUrl(Song song);
        List<Song> SearchSongs(string keyword, int page, int pageSize);
    }
}