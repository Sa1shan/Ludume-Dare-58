using UnityEngine;
using System.Collections;

public class SquareController : MonoBehaviour
{
    public enum SquareType { Q, W, E }
    public SquareType type;  // выставляется в инспекторе на каждом из 3 префабов

    private QWEGame game;
    private float lifetime = 2f;

    public void Initialize(QWEGame parent, float timeout)
    {
        game = parent;
        lifetime = timeout;
        StartCoroutine(LiveAndDie());
    }

    private IEnumerator LiveAndDie()
    {
        yield return new WaitForSeconds(lifetime);
        if (game != null)
            game.OnSquareTimedOut(gameObject);
    }

    public bool IsCorrectKey(KeyCode key)
    {
        switch (type)
        {
            case SquareType.Q: return key == KeyCode.Q;
            case SquareType.W: return key == KeyCode.W;
            case SquareType.E: return key == KeyCode.E;
        }
        return false;
    }
}