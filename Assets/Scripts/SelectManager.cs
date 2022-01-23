using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SelectManager : MonoBehaviour
{
    [Tooltip("The camera used for highlighting")]
    public Camera selectCam;
    [Tooltip("The rectangle to modify for selection")]
    public RectTransform SelectingBoxRect;

    private Rect SelectingRect;
    private Vector3 SelectingStart;

    [Tooltip("Changes the minimum square before selecting characters. Needed for single click select")]
    public float minBoxSizeBeforeSelect = 10f;
    public float selectUnderMouseTimer = 0.1f;
    private float selectTimer = 0f;

    private bool selecting = false;

    public List<SelectableCharacter> selectableChars = new List<SelectableCharacter>();
    private List<SelectableCharacter> selectedArmy = new List<SelectableCharacter>();

    private void Awake() {
        //Vérifie si le gestionnaire est placé sur l'image pour le selectionner
        if (!SelectingBoxRect) {
            SelectingBoxRect = GetComponent<RectTransform>();
        }

        //Recherche tous les objets ayant le script SelectableCharacter
        //Puis convertis en lsite
        SelectableCharacter[] chars = FindObjectsOfType<SelectableCharacter>();
        for (int i = 0; i <= (chars.Length - 1); i++) {
            selectableChars.Add(chars[i]);
        }
    }

    void Update() {
        if (SelectingBoxRect == null) {
            Debug.LogError("There is no Rect Transform to use for selection!");
            return;
        }

        //L'input pour utilise la sélection
        if (Input.GetMouseButtonDown(0)) {
            ReSelect();

            //Paramètre la zone de sélection
            SelectingStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            SelectingBoxRect.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        } else if (Input.GetMouseButtonUp(0)) {
            selectTimer = 0f;
        }

        selecting = Input.GetMouseButton(0);

        if (selecting) {
            SelectingArmy();
            selectTimer += Time.deltaTime;

            //Vérifie uniquement s'il y a un SelectableCharacter sous la souris
            if (selectTimer <= selectUnderMouseTimer) {
                CheckIfUnderMouse();
            }
        } else
            SelectingBoxRect.sizeDelta = new Vector2(0, 0);
    }

    //Réinitialise la sélection
    void ReSelect() {
        for (int i = 0; i <= (selectedArmy.Count - 1); i++) {
            selectedArmy[i].TurnOffSelector();
            selectedArmy.Remove(selectedArmy[i]);
        }
    }

    //Vérifie sur le déplacement de la souris sur l'écran
    //Déplace le pivot de l'UI en fonction de la direction vers laquelle la souris se dirige par rapport à son point de départ
    void SelectingArmy() {
        Vector2 _pivot = Vector2.zero;
        Vector3 _sizeDelta = Vector3.zero;
        Rect _rect = Rect.zero;

        //Contrôle le pivot x
        if (-(SelectingStart.x - Input.mousePosition.x) > 0) 
        {
            _sizeDelta.x = -(SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x;
        } 
        else 
        {
            _pivot.x = 1;
            _sizeDelta.x = (SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x - SelectingBoxRect.sizeDelta.x;
        }

        //Contrôle le pivot y
        if (SelectingStart.y - Input.mousePosition.y > 0) 
        {
            _pivot.y = 1;
            _sizeDelta.y = SelectingStart.y - Input.mousePosition.y;
            _rect.y = SelectingStart.y - SelectingBoxRect.sizeDelta.y;
        } 
        else 
        {
            _sizeDelta.y = -(SelectingStart.y - Input.mousePosition.y);
            _rect.y = SelectingStart.y;
        }

        //Paramètre le pivot 
        if (SelectingBoxRect.pivot != _pivot)
            SelectingBoxRect.pivot = _pivot;

        //Paramètre la taille
        SelectingBoxRect.sizeDelta = _sizeDelta;

        //Finis puis paramètre le rect
        _rect.height = SelectingBoxRect.sizeDelta.x;
        _rect.width = SelectingBoxRect.sizeDelta.y;
        SelectingRect = _rect;

        //Vérifie que la taille de la sélection est plus grande que la taille minimale
        //Tout en vérifiant avec un seul clic
        if (_rect.height > minBoxSizeBeforeSelect && _rect.width > minBoxSizeBeforeSelect) {
            CheckForSelectedCharacters();
        }
    }

    //Vérifie si les characters corrects peuvent être sélectionnés, puis les "sélectionne"
    void CheckForSelectedCharacters() {
        foreach (SelectableCharacter soldier in selectableChars) {
            Vector2 screenPos = selectCam.WorldToScreenPoint(soldier.transform.position);
            if (SelectingRect.Contains(screenPos)) {
                if (!selectedArmy.Contains(soldier))
                    selectedArmy.Add(soldier);

                soldier.TurnOnSelector();
            } else if (!SelectingRect.Contains(screenPos)) {
                soldier.TurnOffSelector();

                if (selectedArmy.Contains(soldier))
                    selectedArmy.Remove(soldier);
            }
        }
    }

    //Vérifie s'il y a un character sous la souris qui est sur la lsite des SelectableCharacters
    void CheckIfUnderMouse() {
        RaycastHit hit;
        Ray ray = selectCam.ScreenPointToRay(Input.mousePosition);

        //Raycast depuis la souris et sélectionne le personnage lors du clic
        if (Physics.Raycast(ray, out hit, 100f)) {
            if (hit.transform != null) {
                SelectableCharacter selectChar = hit.transform.gameObject.GetComponentInChildren<SelectableCharacter>();
                if (selectChar != null && selectableChars.Contains(selectChar)) {
                    selectedArmy.Add(selectChar);
                    selectChar.TurnOnSelector();
                }
            }
        }
    }
}
