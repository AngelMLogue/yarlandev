using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movimiento : MonoBehaviour
{
    Rigidbody rb;
    float velocidad = 20f;
    float ejex, ejez;
    bool grounded = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded == true){ 
            rb.AddForce(0f, 5f, 0f, ForceMode.Impulse);
            grounded = false;
        }
        ejex = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        ejez = Input.GetAxis("Vertical") * velocidad * Time.deltaTime;
        transform.Translate(ejex, 0, ejez);
    }

    public void OnCollisionEnter(Collision collision)
    {

        switch(gameObject.tag)
        {
            case "UsuarioMenor":
                switch(collision.collider.tag)
                {
                    case "SalaMaestros":
                        StartCoroutine(PostData(gameObject.tag, collision));
                        break;
                    case "Patio":
                        StartCoroutine(PostData(gameObject.tag, collision));
                        break;
                    case "CuartoConserje":
                        StartCoroutine(PostData(gameObject.tag, collision));
                        break;
                    default:
                        break;
                }
                break;
            case "UsuarioMayor":
                switch(collision.collider.tag)
                {
                    case "BanosAlumnos":
                        StartCoroutine(PostData(gameObject.tag, collision));
                        break;
                    case "CuartoConserje":
                        StartCoroutine(PostData(gameObject.tag, collision));
                        break;
                    default:
                        break;
                }
                break;
            case "UsuarioAutorizado":
                break;
            default:
                break;

        }

        if (collision.collider.tag == "Suelo")
        {
            grounded = true;
        }
    }

    IEnumerator PostData(string dataStr, Collision collision)
    {

        string jsonData = "{\"alerta\":\"Un " + dataStr + " Entró a un área no autorizada\",\"tipoUsuario\":\"" + dataStr + "\",\"zona\":\"" + collision.collider.tag + "\"}";


        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest request = new UnityWebRequest("https://yarlanapirest.azurewebsites.net/api/Mensajes", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            string results = request.downloadHandler.text;
            Debug.Log(results);
        }

        request.Dispose();
    }
}