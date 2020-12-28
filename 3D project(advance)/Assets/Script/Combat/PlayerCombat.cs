using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using RPG.Core;
using UnityEngine;


namespace RPG.Combat
{
    // harusnya nama class jadi combatMechanic 
    // class ini untuk proses semua data yang masuk dari class control 
    public class PlayerCombat : MonoBehaviour , IAction {
        
        [Header("Attack Setting")]
        [SerializeField] float attackRange = 2f;
        [SerializeField] float attactTime = 1f;
        [SerializeField] float attackDamage = 5f;
        

        //variable untuk test code attackComboAnimation();
        //List<string> animationList = new List<string>(new string[] {"attack1AnimTrigger","attack2AnimTrigger" });
        //int comboNumber = 0;
        
        
        // health dari component target bukan component dari player
        Health target;

        // agar player atau musuh langsung serang 
        // bisa pake angka yang lebih dari attack time 
        // pakai mathf.infinity agar langsung buat menjadi lebih besar daro attactTime
        float lastSecondAttack = Mathf.Infinity;

        private void Update()
        {
            // jika ada target dan rangeCalculate nya false maka player bisa jalan
            // jika rangeCalculate true maka player akan berhenti
            // jika target null maka akan di return langsung tampa menjalankan sisanya
            // jika frame ke update maka lastSecond akan menjadi waktu dari time.deltatime
            // jika target sudah mati maka tidak akan melakukan serangan

            lastSecondAttack += Time.deltaTime;

            if(target == null) return;
            if(target.isDead()) return;

            if (!rangeCalculate())
            {
                GetComponent<mover>().moveTo(target.transform.position);
            }
            else
            {
                GetComponent<mover>().cancelAction();
                AttackAnimation();
            }
        }


        private bool rangeCalculate()
        {
            // hitung selisih jarak target dengan player 
            // jika lebih kecil dari range maka akan return true
            return Vector3.Distance(target.transform.position, transform.position) < attackRange;
        }

        public bool canAttack(GameObject combatTarget){

            //jika tidak ada gameObject maka akan return false
            //jika ada maka akan diambil komponen health nya dari gameobject
            // jika ada dan belom mati maka akan return true
            // jika target sudah mati maka akan return false
            if(combatTarget == null) return false;

            Health tempTarget = combatTarget.GetComponent<Health>();
            return tempTarget != null && !tempTarget.isDead();
        }

        public void attack(GameObject target){
            // untuk memhentikan current action dan lalu menganti action menjadi action class ini
            // lalu memulai action di class ini 
            // memasukan data target dari gameobject ke dalam target di class ini dengan mengambil komponen health karena dataype untuk target pada class ini adalah health
            // mencari target yang memiliki component CombatTarget
            GetComponent<ActionScheduler>().StartAction(this);
            this.target = target.GetComponent<Health>();
        }


        public void cancelAction()
        {
            //dari interface
            cancelAttack();
        }


        #region animation
        //animation
        public void cancelAttack(){
            // reset target ke null
            // jika dibatalkan maka animasi serang akan di reset ( bug kecil )
            GetComponent<Animator>().ResetTrigger("attackAnimTrigger");
            GetComponent<Animator>().SetTrigger("cancelAttackTrigger");
            this.target = null;
        }

        private void AttackAnimation()
        {
            // lookat agar player selalu hadap ke target saat melakukan attack
            // jika lastSecondAttack lebih besar dari attack time maka akan melakukan animasi
            //trigger method hit yang sudah ditaruk kedalam animasi
            // jika sudah melakukan animasi maka lastSecondAttack akan diubah ke 0 agar menjadi lebih kecil dari attack time
            // method ini bisa dipake dalam game fps seperti firerate pada senjata atau bisa dipakai sebagai cooldown dari suatu skill
            // jika menyerang maka cancelattack akan di reset ( bug kecil )
            transform.LookAt(target.transform);
            if (lastSecondAttack > attactTime)
            {
                GetComponent<Animator>().ResetTrigger("cancelAttackTrigger");
                GetComponent<Animator>().SetTrigger("attackAnimTrigger");
                lastSecondAttack = 0;
            }

            
        }


        // test code untuk combo
        // private void ComboAttackAnimation()
        // {
        //     if (lastSecondAttack > attactTime)
        //     {
        //         while (comboNumber != animationList.Count)
        //         {
        //             GetComponent<Animator>().SetTrigger(animationList[comboNumber]);
        //             comboNumber++;
        //             lastSecondAttack = 0;
        //         }
        //         if (comboNumber == animationList.Count)
        //         {
        //             comboNumber = 0;
        //         }
        //     }
        // }

        void Hit(){
            // jika tidak ada target maka tidak akan melakukan method takeDamage (untuk menhindari null object exception doang (bug))
            // jika animasi sudah sampai di durasi hit baru melakukan damage
            // mengambil komponen health dari target untuk melakukan method takeDamage
            if(target == null) return;
            target.takeDamage(attackDamage);
        }

        #endregion
    }

    
}