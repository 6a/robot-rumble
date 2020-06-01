using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace RR.Facilitators.UI
{
    public class Scoreboard : MonoBehaviour, IScoreboard
    {
        [SerializeField] private PlayerScoreRow[] _playerScoreRows = null;
        [SerializeField] private float _rowHeight = -50f;

        private SortedDictionary<int, IPlayerScoreRow> _playerScoreRowInterfaces = new SortedDictionary<int, IPlayerScoreRow>();
        bool _intitialized = false;

        private void FixedUpdate() 
        {
            if (!_intitialized)
            {
                TrySetup();
            }
            else if (PhotonNetwork.playerList != null)
            {
                var sortedPlayers = from player in PhotonNetwork.playerList
                                    orderby player.GetScore() descending, player.NickName ascending 
                                    select player;
                
                var currentRank = 1;
                var HighestHealth = sortedPlayers.ElementAt(0).GetScore();
                var nextPlaceGap = 0;
                var position = 1;

                var updated = new HashSet<int>();
                foreach (var player in sortedPlayers)
                {
                    var score = player.GetScore();
                    if (score < HighestHealth)
                    {
                        currentRank += nextPlaceGap;
                        HighestHealth = score;

                        nextPlaceGap = 1;
                    }
                    else
                    {
                        nextPlaceGap++;
                    }

                    _playerScoreRowInterfaces[player.ID].UpdateValues(score, currentRank);
                    _playerScoreRowInterfaces[player.ID].SetYOffset((position - 1) * _rowHeight);

                    position++;

                    updated.Add(player.ID);
                }

                if (updated.Count < 4)
                {
                    foreach (var row in _playerScoreRowInterfaces)
                    {
                        if (!updated.Contains(row.Key))
                        {
                            _playerScoreRowInterfaces[row.Key].UpdateValues(0, 0);
                            _playerScoreRowInterfaces[row.Key].SetYOffset((position - 1) * _rowHeight);
                            _playerScoreRowInterfaces[row.Key].SetAsDisconnected();
                            position++;
                        }
                    }
                }
            }
        }

        private void TrySetup()
        {
            if (PhotonNetwork.inRoom)
            {
                object gsObject = null;
                var gameStarted = false;
                var found = PhotonNetwork.room.CustomProperties.TryGetValue("gamestarted", out gsObject);
                if (found) gameStarted = (bool)gsObject;

                if (gameStarted)
                {
                    var photonPlayers = from player in PhotonNetwork.playerList orderby player.ID ascending select player;
                    var localPlayerID = PhotonNetwork.player.ID;

                    for (int i = 0; i < _playerScoreRows.Length; i++)
                    {
                        var row = _playerScoreRows[i] as IPlayerScoreRow;
                        var name = "disconnected";
                        var score = 0;
                        var photonID = 0;
        
                        if (i < photonPlayers.Count())
                        {
                            name = photonPlayers.ElementAt(i).NickName;
                            score = photonPlayers.ElementAt(i).GetScore();
                            photonID = photonPlayers.ElementAt(i).ID;
                        }
                        else
                        {
                            photonID = -10000 + (i * -10000);
                            row.SetAsDisconnected();
                        }

                        row.SetName(name);
                        row.UpdateValues(score, 0);

                        if (photonID == localPlayerID)
                        {
                            row.SetAsLocalPlayerRow();
                        }
                        else
                        {
                            row.SetAsOpponentRow();
                        }

                        _playerScoreRowInterfaces.Add(photonID, row);
                    }

                    _intitialized = true;
                }
            }
        }

        public void Reset()
        {
            _intitialized = false;
            _playerScoreRowInterfaces.Clear();
        }
    }
}