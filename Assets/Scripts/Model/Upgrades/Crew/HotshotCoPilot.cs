﻿using Abilities;
using GameModes;
using Ship;
using Upgrade;
using Tokens;

namespace UpgradesList
{
    public class HotshotCoPilot : GenericUpgrade
    {
        public HotshotCoPilot() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Hotshot Co-pilot";
            Cost = 4;

            UpgradeAbilities.Add(new HotshotCoPilotAbility());
        }
    }
}

namespace Abilities
{
    public class HotshotCoPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckAttackAbility;
            HostShip.OnAttackStartAsDefender += DefenceAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckAttackAbility;
            HostShip.OnAttackStartAsDefender -= DefenceAbility;
        }

        private void CheckAttackAbility()
        {
            if (Combat.ChosenWeapon is PrimaryWeaponClass) AssignCondition(Combat.Defender);
        }

        private void DefenceAbility()
        {
            AssignCondition(Combat.Attacker);
        }

        private void AssignCondition(GenericShip ship)
        {
            Messages.ShowInfo("Hotshot Co-pilot effect is active");

            ship.Tokens.AssignCondition(new Conditions.HotshotCoPilotCondition(ship));

            ship.OnTryConfirmDiceResults += DisallowIfHasFocusToken;
            ship.OnTokenIsSpent += CheckRemoveCondition;
            ship.OnAttackFinish += RemoveCondition;
        }

        private void CheckRemoveCondition(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(FocusToken)) RemoveCondition(ship);
        }

        private void RemoveCondition(GenericShip ship)
        {
            Messages.ShowInfo("Hotshot Co-pilot effect is not active");

            ship.OnTryConfirmDiceResults -= DisallowIfHasFocusToken;
            ship.OnAttackFinish -= RemoveCondition;
            ship.OnTokenIsSpent -= CheckRemoveCondition;

            ship.Tokens.RemoveCondition(typeof(Conditions.HotshotCoPilotCondition));
        }

        private void DisallowIfHasFocusToken(ref bool result)
        {
            GenericShip currentShip = null;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    currentShip = Combat.Attacker;
                    break;
                case CombatStep.Defence:
                    currentShip = Combat.Defender;
                    break;
                default:
                    break;
            }

            if (currentShip.Tokens.HasToken(typeof(FocusToken)))
            {
                Messages.ShowError("Cannot confirm results - must spend focust token!");
                result = false;
            }
        }
    }
}

namespace Conditions
{
    public class HotshotCoPilotCondition : GenericToken
    {
        public HotshotCoPilotCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new UpgradesList.HotshotCoPilot().ImageUrl;
        }
    }
}