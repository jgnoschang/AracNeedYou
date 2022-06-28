using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public GameObject TeiaControler, player, cameraPrincipal;
    public GameObject[] posicoes;
    public int indice = 0;
    public float velocidadeDeMovimento = 2;
    private RaycastHit hit;

    void start()
    {
    }

    void FixedUpdate()//taxa de quadro fixa
    {

        if (!Physics.Linecast(player.transform.position, cameraPrincipal.transform.position))//se nao tem nada atrapalhando a vista (entre o player e a posição)
        {
            transform.position = Vector3.Lerp(transform.position, posicoes[indice].transform.position, velocidadeDeMovimento * Time.deltaTime); // a camera segue somente se não haver nada(interrompendo a linha) entre o ponto da posição 1 a cabeça do player
            //Debug.DrawLine(TeiaControler.transform.position, posicoes[indice].transform.position);
        }
        else if (Physics.Linecast(player.transform.position, cameraPrincipal.transform.position, out hit))//se colidiu, pega o ponto que a linha colidiu na variavel "hit" e faz com que a camera vai para este ponto com vista para o player
        { 
            transform.position = Vector3.Lerp(transform.position, hit.point, velocidadeDeMovimento * 2 * Time.deltaTime);
            //Debug.DrawLine(TeiaControler.transform.position, hit.point);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (indice == 0)
                indice = 1;
            else
                indice = 0;
        }
    }
}
