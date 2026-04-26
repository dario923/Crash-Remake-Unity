using UnityEngine;

public class Settings 
{
    // Implementación de Singleton segura y eficiente
    private static class SingletonLoader {
        public static readonly Settings instance = new Settings();
    }
    
    public static Settings getInstance() {
        return SingletonLoader.instance;
    }

    private AudioSource m_MusicSource;
    private AudioSource m_SoundSource;

    // Propiedades para controlar el volumen
    public float MusicVolume {
        get { return m_MusicSource != null ? m_MusicSource.volume : 0.5f; }
        set { if (m_MusicSource != null) m_MusicSource.volume = value; }
    }

    public float SoundVolume {
        get { return m_SoundSource != null ? m_SoundSource.volume : 1.0f; }
        set { if (m_SoundSource != null) m_SoundSource.volume = value; }
    }

    public int HighScore { get; set; }

    // Método para inicializar y cargar datos
    public void Load(AudioSource music, AudioSource sound) {
        m_MusicSource = music;
        m_SoundSource = sound;

        // Recuperamos los valores guardados o usamos por defecto
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSound = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        
        MusicVolume = savedMusic;
        SoundVolume = savedSound;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        
        Debug.Log("<color=green>Settings:</color> Datos cargados correctamente.");
    }

    // Método para guardar en disco
    public void Save() {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SoundVolume", SoundVolume);
        PlayerPrefs.SetInt("HighScore", HighScore);
        
        // Fuerza el guardado físico en el disco
        PlayerPrefs.Save();
        Debug.Log("<color=yellow>Settings:</color> Datos guardados en PlayerPrefs.");
    }
}