using Kaypic_Web3.Data;
using Kaypic_Web3.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Kaypic_Web3.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessagingDbContext _msg;

        public ChatHub(MessagingDbContext msg)
        {
            _msg = msg;
        }

        // Création d'un groupe
        public async Task<int> CreateGroup(string titre, List<int> participantIds)
        {
            var creatorId = Context.GetHttpContext().Session.GetInt32("UtilisateurID");
            if (creatorId == null) throw new Exception("Utilisateur non connecté");

            var conversation = new Conversation
            {
                Titre = titre,
                date_creation = DateTime.Now,
                status = Status.Active
            };
            _msg.Conversations.Add(conversation);
            await _msg.SaveChangesAsync();

            var allParticipants = participantIds.ToList();
            allParticipants.Add(creatorId.Value);

            foreach (var userId in allParticipants.Distinct())
            {
                _msg.ConversationUtilisateurs.Add(new ConversationUtilisateur
                {
                    ConversationId = conversation.Id,
                    Id_utilisateur = userId,
                    ajouter_a = DateTime.Now
                });
            }
            await _msg.SaveChangesAsync();

            await Clients.All.SendAsync("GroupCreated", conversation.Id, titre);

            return conversation.Id;
        }

        //rejoindre un groupe par une conversation
        public Task JoinConversation(int conversationId)
            => Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

        //quitte
        public Task LeaveConversation(int conversationId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());

        //envoyer un message
        public async Task SendMessage(int conversationId, int senderId, string senderNom, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            //enregistre message dans bd
            var entity = new HistoriqueMessages
            {
                ConversationId = conversationId,
                IdEnvoyeur = senderId,
                NomEnvoyeur = senderNom,
                Message = message,
                DateEnvoi = DateTime.Now
            };

            _msg.HistoriqueMessages.Add(entity);
            await _msg.SaveChangesAsync();

            //diffuse le message a tous les membre de la conversation
            await Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    messageId = entity.Id,
                    conversationId = conversationId,
                    senderNom = senderNom,
                    senderId = senderId,
                    message = message,
                    dateEnvoi = entity.DateEnvoi,
                    reactions = entity.Reactions
                        .GroupBy(r => r.Emoji)
                        .Select(g => new { Emoji = g.Key, Count = g.Count() })
                        .ToList()
                });
        }

        public async Task SendFile(int senderId, string fileName, string fileUrl)
        {
            await Clients.All.SendAsync("ReceiveFile", senderId, fileName, fileUrl);
        }

        public async Task AddReaction(int messageId, int userId, string emoji)
        {
            var existing = await _msg.Reactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.Emoji == emoji);

            if (existing != null)
            {
                _msg.Reactions.Remove(existing);
            }
            else
            {
                _msg.Reactions.Add(new Reaction
                {
                    MessageId = messageId,
                    UserId = userId,
                    Emoji = emoji
                });
            }

            await _msg.SaveChangesAsync();

            var message = await _msg.HistoriqueMessages
                .Include(m => m.Reactions)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message != null)
            {
                var reactionsSummary = message.Reactions
                    .GroupBy(r => r.Emoji)
                    .Select(g => new { Emoji = g.Key, Count = g.Count() })
                    .ToList();

                await Clients.Group(message.ConversationId.ToString())
                    .SendAsync("UpdateReaction", messageId, reactionsSummary);
            }

            Console.WriteLine($"Reaction added: message={messageId}, user={userId}, emoji={emoji}");
        }



        public async Task StartAudioCall(int conversationId, int callerId, string callerNom)
        {
            var appel = new HistoriqueAppels
            {
                ConversationId = conversationId,
                Id_appelleur = callerId,
                startedAt = DateTime.Now
            };

            _msg.HistoriqueAppels.Add(appel);
            await _msg.SaveChangesAsync();

            //Notifie les autre membres de la conversation
            await Clients.Group(conversationId.ToString())
                .SendAsync("IncomingAudioCall", new
                {
                    conversationId,
                    callerId,
                    callerNom,
                    startedAt = DateTime.Now
                });
        }

        //Un utilisateur accepte l'appel
        public async Task AcceptAudioCall(int conversationId, int userId, string userNom)
        {
            await Clients.Group(conversationId.ToString())
                .SendAsync("AudioCallAccepted", new
                {
                    conversationId,
                    userId,
                    userNom,
                    acceptedAt = DateTime.Now
                });
        }

        //Raccrocher
        public async Task EndAudioCall(int conversationId, int userId)
        {
            var appel = await _msg.HistoriqueAppels
            .Where(a => a.ConversationId == conversationId && a.endedAt == null)
            .OrderByDescending(a => a.startedAt)
            .FirstOrDefaultAsync();

            if (appel != null)
            {
                appel.endedAt = DateTime.Now;
                await _msg.SaveChangesAsync();
            }
            await Clients.Group(conversationId.ToString())
                .SendAsync("AudioCallEnded", new
                {
                    conversationId,
                    userId,
                    endedAt = DateTime.Now
                });
        }

        //audio
        public async Task SendWebRtcOffer(int conversationId, int fromUserId, string sdpOffer)
        {
            await Clients.OthersInGroup(conversationId.ToString())
                .SendAsync("ReceiveWebRtcOffer", new
                {
                    conversationId,
                    fromUserId,
                    sdpOffer
                });
        }
        public async Task SendWebRtcAnswer(int conversationId, int fromUserId, string sdpAnswer)
        {
            await Clients.OthersInGroup(conversationId.ToString())
                .SendAsync("ReceiveWebRtcAnswer", new
                {
                    conversationId,
                    fromUserId,
                    sdpAnswer
                });
        }
        public async Task SendIceCandidate(int conversationId, int fromUserId, string candidate)
        {
            await Clients.OthersInGroup(conversationId.ToString())
                .SendAsync("ReceiveIceCandidate", new
                {
                    conversationId,
                    fromUserId,
                    candidate
                });
        }
    }
}