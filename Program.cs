using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NotionOffline;

class Program
{
    static void Main(string[] args)
    {
        string folder = "NotionFiles/Private & Shared";
        string filename = "Explore 261340b17c4980f9aa9be784a359d0d4.html";
        string fullPath = Path.Combine(folder, filename);
        string[] lines = File.ReadAllLines(fullPath);
        DownloadAllFilesInHtml(lines, folder);
        ReplaceHttpsWithLocalFile(filename, folder);
    }

    static void DownloadAllFilesInHtml(string[] lines, string folder)
    {
        foreach (string line in lines)
        {
            foreach (Match match in Regex.Matches(line, @"https://[^\s""']+"))
            {
                string url = match.Value;
                string imgFilename = GetImageFileName(url);
                DownloadFileFromUrl(url, imgFilename, folder);
            }
        }
    }

    static void DownloadFileFromUrl(string url, string filename, string folder)
    {
        string fullPath = Path.Combine(folder, filename);

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(url, fullPath);
        }
        Console.WriteLine($"Saved URL: {url} to {folder} as {filename}");
    }

    static void ReplaceHttpsWithLocalFile(string filename, string folder)
    {
        string fullPath = Path.Combine(folder, filename);
        string html = File.ReadAllText(fullPath);

        string updatedHtml = Regex.Replace(html, @"https://[^\s""']+", new MatchEvaluator(ReplaceUrlWithFilename));

        string updatedPath = Path.Combine(folder, "Updated_" + filename);

        File.WriteAllText(updatedPath, updatedHtml);

        Console.WriteLine("Updated HTML saved to " + updatedPath);
    }

    static string ReplaceUrlWithFilename(Match match)
    {
        string url = match.Value;

        string imgFilename = GetImageFileName(url);

        return imgFilename;
    }

    static string GetImageFileName(string url)
    {
        string imgFilename = Path.GetFileName(url);
        if (imgFilename.Contains("fm=jpg"))
        {

            imgFilename = Regex.Match(imgFilename, @"[^?]+").Value;
            imgFilename += ".jpg";
        }
        return imgFilename;
    }
}
