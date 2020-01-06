using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class Pool_Object{
        public Transform transform;
        public bool in_use;
        public int y_direction;

        public Pool_Object(Transform t){ // constructor
            transform = t;
            y_direction = 0;
        }
        public void Use(){
            in_use = true;
        }
        public void Dispose(){
            in_use = false;
        }
    }

    [System.Serializable]
    public struct y_spawn_range{
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int pool_size;
    public float shift_speed;
    public float shift_y_speed;
    public float spawn_rate;

    public y_spawn_range y_range;
    public Vector3 default_spawn_pos;
    public bool spawn_immediate; //partcle prewarm, true for everything except the bird
    public Vector3 immediate_spawn_pos;
    public Vector2 target_aspect_ratio; //aren't being spawned within screen space

    float spawn_timer;
    float target_aspect; //width / height
    Pool_Object[] pool_objects;
    GameManager game;

    void Awake(){
        Configure();
    }

    void Start(){
        game = GameManager.Instance;
    }

    void OnEnable(){
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable(){
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed(){
        for (int i = 0; i < pool_objects.Length; i++){
            pool_objects[i].Dispose();
            pool_objects[i].transform.position = Vector3.one * 1000;
        }
        if (spawn_immediate){
            Spawn_Immediate();
        }
    }

    void Update(){
        if (game.GameOver){
            return;
        }
        Shift();
        Shift_Y();
        spawn_timer += Time.deltaTime;
        if (spawn_timer > spawn_rate){
            Spawn();
            spawn_timer = 0;
        }
    }

    void Configure(){
        target_aspect = target_aspect_ratio.x / target_aspect_ratio.y;
        pool_objects = new Pool_Object[pool_size];
        for (int i = 0; i < pool_objects.Length; i++){
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            pool_objects[i] = new Pool_Object(t);
        }

        if (spawn_immediate){
            Spawn_Immediate();
        }
    }

    void Spawn(){
        Transform t = Get_Pool_Object();
        if (t == null){ // if true, pool_size is too small
            return;
        }
        Vector3 pos = Vector3.zero;
        pos.x = (default_spawn_pos.x * Camera.main.aspect) / target_aspect;
        pos.y = Random.Range(y_range.min, y_range.max);
        t.position = pos;
    }

    void Spawn_Immediate(){
        Transform t = Get_Pool_Object();
        if (t == null){ // if true, pool_size is too small
            return;
        }
        Vector3 pos = Vector3.zero;
        pos.x = (immediate_spawn_pos.x * Camera.main.aspect) / target_aspect;
        pos.y = Random.Range(y_range.min, y_range.max);
        t.position = pos;
        Spawn();
    }

    void Shift(){
        for (int i = 0; i < pool_objects.Length; i++){
            pool_objects[i].transform.localPosition += -Vector3.right * shift_speed * Time.deltaTime;
            Check_Dispose(pool_objects[i]);
        }
    }

    void Check_Dispose(Pool_Object pool_objects){ //parallel axis offscreen
        if (pool_objects.transform.position.x < (-default_spawn_pos.x * Camera.main.aspect) / target_aspect){
            pool_objects.Dispose();
            pool_objects.transform.position = Vector3.one * 1000;
        }
    }

    Transform Get_Pool_Object(){
        for (int i = 0; i < pool_objects.Length; i++){
            if (!pool_objects[i].in_use){
                pool_objects[i].Use();
                pool_objects[i].y_direction = Random.Range(0, 2);
                return pool_objects[i].transform;
            }
        }
        return null;
    }

    void Shift_Y(){
        for (int i = 0; i < pool_objects.Length; i++){
            if (pool_objects[i].transform.position.y < y_range.max && pool_objects[i].transform.position.y > y_range.min){
                if (pool_objects[i].y_direction == 0){
                    pool_objects[i].transform.localPosition += Vector3.down * shift_y_speed * Time.deltaTime;
                }
                else{
                    pool_objects[i].transform.localPosition += Vector3.up * shift_y_speed * Time.deltaTime;
                }
            }
            else if (pool_objects[i].transform.position.y > y_range.max){
                pool_objects[i].y_direction = 0;
                pool_objects[i].transform.localPosition += Vector3.down * shift_y_speed * Time.deltaTime;
            }
            else{
                pool_objects[i].y_direction = 1;
                pool_objects[i].transform.localPosition += Vector3.up * shift_y_speed * Time.deltaTime;
            }
        } 
    }
}
