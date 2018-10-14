using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;


public class Persistance : MonoBehaviour {
    public string filename = "PlayerInfo"; 
    [Serializable]
    public class DataObject {
        public DataObject(string n, object d) {
            name = n;
            data = d;
        }
        public string name;
        public object data;
    }
    Dictionary<string, DataObject> data = new Dictionary<string, DataObject>();
    public static Persistance instance;
    private void Awake() {
        instance = this;
    }

    public void WriteData(string name, object data) {
        WriteData(new DataObject(name, data));
    }

    void WriteData(DataObject d) {
        if (data.ContainsKey(d.name)) {
            data[d.name] = d;
        } else {
            data.Add(d.name, d);
        }
        Save();
    }

    public T ReadData<T>(string key, T defaultValue) {
        Load();
        if (data.ContainsKey(key)) {
            return (T) data[key].data;
        } else {
            return defaultValue;
        }
    }


    void Load() {
        string path = Application.persistentDataPath + "/" + filename;
        if (File.Exists(path)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            data = (Dictionary<string, DataObject>)bf.Deserialize(file);
            file.Close();
        }
    }

    void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + filename, FileMode.OpenOrCreate);
        bf.Serialize(file, data);
        file.Close();
    }
}
