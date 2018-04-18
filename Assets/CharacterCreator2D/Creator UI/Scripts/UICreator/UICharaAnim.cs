using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UICharaAnim : MonoBehaviour
    {
        /// <summary>
        /// UI Text displaying current animation's name.
        /// </summary>
        public Text animName;

        private UICreator _uicreator;
        private int _sanim;
        private List<string> _animname;

        void Awake()
        {
            _uicreator = this.transform.GetComponentInParent<UICreator>();
            _sanim = 0;
            _animname = new List<string>();

            Animator anim = _uicreator.character.GetComponent<Animator>();
            string curranim = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
            {
                _animname.Add(anim.runtimeAnimatorController.animationClips[i].name);
                if (anim.runtimeAnimatorController.animationClips[i].name == curranim)
                    _sanim = i;
            }

            selectAnimation(_sanim);
        }

        /// <summary>
        /// Select next animation.
        /// </summary>
        public void ShiftUp()
        {
            if (_sanim >= _animname.Count - 1)
                selectAnimation(0);
            else
                selectAnimation(_sanim + 1);
        }

        /// <summary>
        /// Select previous animation.
        /// </summary>
        public void ShiftDown()
        {
            if (_sanim <= 0)
                selectAnimation(_animname.Count - 1);
            else
                selectAnimation(_sanim - 1);
        }

        private void selectAnimation(int index)
        {
            if (index < 0 || index >= _animname.Count)
                return;

            _sanim = index;
            _uicreator.character.GetComponent<Animator>().Play(_animname[_sanim]);
            animName.text = _animname[_sanim];
        }
    }
}