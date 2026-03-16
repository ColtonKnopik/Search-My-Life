using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SearchMyLife.Api.Models;
using SearchMyLife.Api.Services;

namespace SearchMyLife.Api.Data;

public static class DbSeeder
{
    private static readonly Guid SeedUserId = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        var user = new User
        {
            Id = SeedUserId,
            Email = "merchant@demo.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234!"),
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var entries = BuildEntries(SeedUserId);
        db.JournalEntries.AddRange(entries);
        await db.SaveChangesAsync();
    }

    public static async Task SeedEmbeddingsAsync(
        AppDbContext db,
        IAiService aiService,
        IVectorSearchService vectorSearchService,
        ILogger logger)
    {
        var entries = await db.JournalEntries
            .Where(e => e.UserId == SeedUserId && e.Summary != null)
            .ToListAsync();

        if (entries.Count == 0)
            return;

        logger.LogInformation("Seeding embeddings for {Count} entries...", entries.Count);

        foreach (var entry in entries)
        {
            try
            {
                var tags = DeserializeTags(entry.Tags);
                var embeddingText = entry.Summary + " " + string.Join(" ", tags);
                var embedding = await aiService.EmbedAsync(embeddingText);
                await vectorSearchService.UpsertEmbeddingAsync(entry.Id, entry.UserId, embedding);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to seed embedding for entry {EntryId}.", entry.Id);
            }
        }

        logger.LogInformation("Embedding seeding complete.");
    }

    private static string[] DeserializeTags(string? tagsJson)
    {
        if (string.IsNullOrWhiteSpace(tagsJson))
            return [];
        try { return JsonSerializer.Deserialize<string[]>(tagsJson) ?? []; }
        catch { return []; }
    }

    private static List<JournalEntry> BuildEntries(Guid userId)
    {
        var now = DateTime.UtcNow;

        return new List<JournalEntry>
        {
            Entry(userId, now.AddDays(-182),
                "The Day I First Saw Her",
                "I scarce know how to write this, for my hand still trembles. She came to my stall at market — Lady Elara, daughter of Lord Aldric himself. She wished to see my finest Florentine cloth, the bolt of deep crimson I had kept for nobility. Her eyes were the colour of winter sky. She spoke to me as though I were a real man, not merely a tradesman beneath her notice. She touched the fabric with such delicacy that I nearly forgot to name a price. I think I charged her far too little. I do not care. God help me.",
                "excited", 0.82,
                "I met Lady Elara for the first time at the market. She bought my finest cloth. I am ruined.",
                new[] { "Elara", "market", "first meeting", "cloth" }),

            Entry(userId, now.AddDays(-178),
                "The Tax Collector Came",
                "Bernard the tax collector arrived this morning before I had even opened the shutters. He says I owe three additional shillings on the autumn tally — a dispute over whether dried goods count as perishable. Three shillings! That is nearly what I earn in a good week. I argued until his face went red, but his seal outranks my words. I paid it. I had to draw from the strongbox I keep beneath the floorboard, the one meant for next spring's wool order. This puts me behind by weeks. I could barely eat tonight.",
                "stressed", -0.71,
                "The tax collector took three shillings I could not spare. My savings are set back.",
                new[] { "taxes", "money troubles", "shop" }),

            Entry(userId, now.AddDays(-174),
                "I Cannot Stop Thinking of Her",
                "It has been eight days since she came to my stall. I have thought of almost nothing else, which is a great problem when one is trying to keep ledgers accurate. I made an error in the accounts today — wrote down twelve yards of broadcloth as twenty. My apprentice Thomas caught it before it reached the customer. He looked at me strangely. I pretended to be tired. She is betrothed to no one yet, they say. But she is Lord Aldric's daughter. What am I? I sell cloth. My father sold cloth. His father before that. The distance between us is not measured in streets.",
                "anxious", -0.38,
                "I cannot concentrate on the shop. Lady Elara occupies all my thoughts.",
                new[] { "Elara", "longing", "ledgers", "class" }),

            Entry(userId, now.AddDays(-170),
                "A Fine Day of Trade",
                "Today was among the best trading days I have had in half a year. A merchant from the guildhall ordered forty yards of undyed linen for a commission going north to the abbey. Forty yards! I did not even have to haggle him down. He paid full price without a word. I sold out my stock of Venetian buttons before noon and had to turn away two ladies who wanted more. By evening I had taken in more coin than the last fortnight combined. I should be joyful. And I am — I am grateful for it, truly. But when I locked the door and sat alone with my ledger, I found myself wondering if Lady Elara ever thinks of the merchant who charged her too little for crimson cloth.",
                "grateful", 0.61,
                "My best trading day in months. And yet I still thought of her.",
                new[] { "good trade", "linen", "guild", "Elara" }),

            Entry(userId, now.AddDays(-165),
                "Her Father's Man Spoke to Me",
                "One of Lord Aldric's household guards came to the market today — not to buy anything. He stood at my stall for a moment, looking at nothing in particular, and then said quietly: 'The young lady does not require cloth at present.' That was all. He left. I stood there with a bolt of tawny wool in my arms for what felt like a very long time. I had not done anything. I had not even spoken to her since that first day. How did they know? Did she mention me? Did someone see me watching the manor gate last Thursday evening? I was only walking that road. It means nothing to walk a road.",
                "anxious", -0.65,
                "A guard from Lord Aldric's household warned me away without a single direct word.",
                new[] { "warning", "Lord Aldric", "Elara", "fear" }),

            Entry(userId, now.AddDays(-161),
                "I Bought the Silk Anyway",
                "The Genoese trader Matteo came through town with a bolt of blue silk — pale, like shallow water over white sand — the most beautiful piece of fabric I have seen in my years of this trade. The price was criminal. I bought it anyway. I have no one to sell it to at that price. The guild ladies might take interest but they would offer me half what I paid and I would have to accept. I told myself I bought it as an investment. I hung it up in the back room and looked at it for a long time. Her eyes are nearly that colour. I am a fool with a beautiful bolt of silk and no good reason for owning it.",
                "neutral", 0.12,
                "I bought an expensive bolt of pale blue silk with no sensible reason.",
                new[] { "silk", "Elara", "foolishness", "investment" }),

            Entry(userId, now.AddDays(-157),
                "Godwin Is Undercutting Me Again",
                "Godwin of the northern stall has been selling broadcloth at four pence per yard. Four pence. I cannot source cloth at that price let alone sell it. His fabric is inferior — I pulled a sample apart with two fingers — but customers do not always understand quality until the garment falls apart in the second wash. I spoke to Master Henry at the guild and he shrugged. Apparently Godwin's cousin is on the trade council this quarter. I will not lower my prices to match rubbish. I will find a way. Perhaps I can source the dyed wool from the Flemish man who came through in spring. I need to write to him.",
                "stressed", -0.58,
                "My competitor is selling cheap cloth and the guild will not intervene.",
                new[] { "competition", "Godwin", "guild", "prices", "shop" }),

            Entry(userId, now.AddDays(-153),
                "I Saw Her at the Cathedral",
                "Sunday mass. I arrived early as I always do and found a place three rows behind the left column. Then she came in with her household. Lady Elara. She wore a deep green cloak with fur at the collar and she looked like something painted on a chapel wall. She did not see me. She knelt and she prayed and I watched the candlelight move across her hair and I entirely forgot to pray myself. Father Edmund would be displeased with me if he knew where my mind was. I said an extra Paternoster afterward out of guilt. It did not help much. I walked home thinking about the way she holds her hands when she prays, fingers straight, not interlaced.",
                "calm", 0.29,
                "Saw Lady Elara at Sunday mass. Forgot to pray. Said extra penance.",
                new[] { "Elara", "church", "Sunday", "devotion" }),

            Entry(userId, now.AddDays(-148),
                "I Tried to Write a Letter",
                "I sat up after the shop closed and tried to write a letter. Not to send — I am not so foolish as that — just to write. To put it somewhere outside of my head. I began seven times. The first draft was embarrassing. The second was worse. I used the word 'radiant' twice in the same sentence on the third attempt. By the sixth I had called myself a humble tradesman so many times the phrase lost all meaning. I burned them all in the hearth. The words were always wrong because the true words are too large for letters. I went to bed still smelling of smoke and feeling no better than before.",
                "sad", -0.44,
                "Tried to write her a letter. Burned every draft. Words are insufficient.",
                new[] { "letter", "Elara", "longing", "writing" }),

            Entry(userId, now.AddDays(-144),
                "Rats Got Into the Storeroom",
                "I have rats. Three of them, by the evidence, and they found the bag of dried beans I keep for winter and made considerable work of it. Also they chewed the corner off a bolt of undyed linen which is now unsellable at full price. I set traps but whoever sold me these traps was a charlatan. Thomas suggested I get a cat. The woman next door has cats, four of them, and they are apparently surplus. I will ask her tomorrow. What an entry this is to write. Rats and damaged linen and borrowed cats. Meanwhile somewhere across this town Lady Elara dines in a hall with candles and silver and I am hunting rats with a broom.",
                "stressed", -0.55,
                "Rats destroyed supplies and damaged a bolt of linen. Asked about borrowing a cat.",
                new[] { "rats", "storeroom", "shop damage", "linen" }),

            Entry(userId, now.AddDays(-139),
                "She Smiled",
                "She smiled at me. At the market. She walked past with her maid and she looked at my stall and then she looked at me and she smiled. Not a polite nothing-smile. A real one, a brief bright thing, as though she was remembering something pleasant. I do not know what she could be remembering that involves me. I managed to nod. I think it was a nod. I may have simply moved my head in a vague manner. Thomas was watching and afterward said, 'Was that not Lord Aldric's daughter?' and I said I had not noticed. He absolutely did not believe me. I have been useless for the rest of the day. I keep replaying the smile. I will probably keep replaying it for a month.",
                "excited", 0.91,
                "Lady Elara smiled at me at the market. I have thought of nothing else since.",
                new[] { "Elara", "smile", "market", "hope" }),

            Entry(userId, now.AddDays(-134),
                "They Say She Will Be Betrothed",
                "I heard it first from Agnes who sells ribbons. Then from the baker. Then from Thomas who heard it in the alehouse. Lord Aldric is in correspondence with a family north of here, a minor barony, regarding a match for Lady Elara. The man is said to be thirty-five and twice widowed. I sat very still on my stool for a while after Thomas told me this. Then I went on folding cloth because there was nothing else to do. She is not mine. She has never been mine. She never could be. I know all of this. Knowing it has never made it less true and it does not make it less painful now. I am a fool who sells cloth. I always knew this ending.",
                "sad", -0.87,
                "Rumours say Lady Elara is being arranged to marry a northern lord.",
                new[] { "Elara", "betrothal", "heartbreak", "rumour" }),

            Entry(userId, now.AddDays(-130),
                "On What Separates Us",
                "I have been thinking about what it means to be born where I was born. My father was a good man. Honest. He built this shop with his own hands and I have kept it for seventeen years. I do not owe debts I cannot manage. I keep my word. I cheat no one. Is that not worth something? In the eyes of the Church all souls are equal. But in the eyes of Lord Aldric's household, I am no one. I am a name on a trade record and a stall at the market. Elara was born into stone walls and heraldry and I was born into bolts of cloth and wool dust. I do not resent her for it. I resent nothing about her. I only resent the distance and the fact that it cannot be crossed.",
                "sad", -0.72,
                "Reflecting on the gulf of class between us. My honest trade counts for nothing.",
                new[] { "class", "reflection", "injustice", "Elara" }),

            Entry(userId, now.AddDays(-126),
                "Thomas Left",
                "My apprentice Thomas has gone. He got an offer from the tanner across town, better wage, more prospects he said. He was decent enough and gave me a week's notice. I thanked him and paid out what I owed. Now I am alone in the shop again. Managing stock, sales, and accounts alone is difficult. I will need to find someone. The guild has a list of apprentices seeking placement but the good ones get placed fast. I should have treated Thomas better. I gave him fair wage but perhaps not enough work worth doing. The shop feels quiet without him rattling around in the storeroom. Even the rats have been quiet since I got Agnes's cat.",
                "sad", -0.49,
                "Thomas my apprentice left for a better position. I am alone in the shop again.",
                new[] { "Thomas", "apprentice", "alone", "shop" }),

            Entry(userId, now.AddDays(-121),
                "Almost",
                "She was at the cloth merchant's row today without her maid, which is unusual. She stopped at Godwin's stall first — his cheap cloth, which I hope disappointed her — and then she came toward mine. I was holding a bolt of green serge and I put it down and I prepared to speak. I had words ready. Good morning, my lady. I hoped the crimson served you well. Ordinary words, harmless words. But then one of the alderman's sons came up the row and called out to her and she turned and they began talking, and after a moment she walked away with him toward the guild hall. I stood there with my good-morning-my-lady prepared and going nowhere. I put the serge back on the shelf.",
                "anxious", -0.33,
                "She nearly came to my stall alone. An alderman's son intercepted her.",
                new[] { "Elara", "almost", "missed chance", "market" }),

            Entry(userId, now.AddDays(-116),
                "A Good Week",
                "I should record good weeks as faithfully as bad ones. This has been a good week. Sold out of the Norwich wool entirely. Received a commission from the miller's wife for twelve yards of sturdy linen, which is good steady work. The cat — I have named her Vesper — has apparently dealt with the rat problem decisively. I found no further evidence of rats and she sits in the storeroom doorway each morning looking extremely satisfied. I got the accounts straight and I am not as behind as I feared. If next week matches this one I can afford the Flemish wool order before the cold makes the roads impassable. I feel almost like a real merchant.",
                "happy", 0.74,
                "Strong sales week. Wool sold out. Cat resolved the rat situation completely.",
                new[] { "good trade", "wool", "Vesper", "shop" }),

            Entry(userId, now.AddDays(-110),
                "Does She Know I Exist",
                "I have been keeping this journal for some months now and I notice I return to the same question like a cart wheel finding a rut in the road: does she know I exist? She has come to my stall once. She smiled at me once. She once came close to stopping at my stall again. That is the whole of it. It is possible that to Lady Elara I am simply the cloth merchant on the east row, one of twenty faces she passes in a market morning. It is possible my name has never entered her thoughts. It is possible that I am building an entire interior world around a woman who has no idea it exists. This thought does not stop me from building it. I wonder what that says about me.",
                "neutral", -0.18,
                "Questioning whether I exist to her at all beyond a brief market transaction.",
                new[] { "Elara", "doubt", "reflection", "existence" }),

            Entry(userId, now.AddDays(-104),
                "I Heard Her Voice Again",
                "I was making deliveries in the upper market when I heard her voice from across the square — she was talking with her maid about something. I could not make out the words but the sound of it stopped me entirely. She was laughing. I have not heard her laugh before and it was the most ordinary extraordinary thing. Just a young woman laughing at something her maid said. I stood behind a cart for a moment longer than was reasonable and then I moved on because a grown man hiding behind a cart is undignified regardless of his reasons. But I kept hearing it all the way back to the shop. Some sounds follow you.",
                "calm", 0.41,
                "Heard Lady Elara laughing across the square. A small ordinary miracle.",
                new[] { "Elara", "voice", "laughter", "market" }),

            Entry(userId, now.AddDays(-98),
                "The Betrothal Confirmed",
                "It is confirmed. Not rumour anymore. Lord Aldric announced the formal terms at the guild hall gathering, which I did not attend but which Master Henry described to me afterward in full detail I did not request. The northern lord. His name is Sir Gideon of something. He has lands and horses and presumably all the correct ancestors. The wedding will be in the spring after next. A year and a half. I have a year and a half of her remaining in this town and then she will be gone north to a stone hall I will never see. I am glad the shop was closed by then. I sat in the back with Vesper in my lap until it was fully dark outside.",
                "sad", -0.93,
                "Elara's betrothal to Sir Gideon is formally announced. The wedding is in spring.",
                new[] { "Elara", "betrothal confirmed", "grief", "Sir Gideon" }),

            Entry(userId, now.AddDays(-91),
                "Work Is the Only Medicine",
                "I have decided not to think about it. I am instead going to think about wool. And linen. And the price differential between Flemish dye and the domestic stuff and whether anyone in this town can actually tell the difference. I have reorganised the storeroom entirely. Vesper supervised. I updated every ledger going back eighteen months. I repaired the broken shelf peg that I have been ignoring since spring. I wrote a letter to the Flemish trader about autumn prices. I am going to build this shop into something worth having. That is something I can actually do. I am going to stop writing about things I cannot change and write about things I can.",
                "neutral", 0.22,
                "Threw myself into work to avoid grief. Reorganised, wrote letters, repaired shelves.",
                new[] { "coping", "work", "shop", "resolution" }),

            Entry(userId, now.AddDays(-84),
                "I Spoke to Her",
                "I spoke to her. Properly. She came to the row again — alone this time, truly alone, no maid, no alderman's sons — and she stopped at my stall and said she had been thinking about that crimson cloth and wondered if I still had any in stock. I did not. I told her so. And then — I do not know exactly how — we talked for perhaps ten minutes. She asked about where the cloth came from. I told her about Florence, about the dye houses, about the ships that bring it north. She listened. She seemed genuinely interested. She said she had never thought about how cloth got to market before, that she supposed she had simply always found it in front of her. She thanked me and left. I forgot to breathe for a while after.",
                "excited", 0.95,
                "We spoke for ten minutes at my stall. She listened. She seemed genuinely interested.",
                new[] { "Elara", "conversation", "breakthrough", "cloth", "hope" }),

            Entry(userId, now.AddDays(-77),
                "She Came Back",
                "She came back. She bought nothing — I think she came to talk. We spoke about the market, about the town, about the northern roads she will travel when she leaves. She asked if I had always been a cloth merchant. I told her about my father, about learning the trade as a boy, about the first time I went to a proper cloth fair at twelve years old and was overwhelmed by the colour of it all. She said that sounded like a fine memory to have. Then she said she wished she had more of that kind of memory. I am not sure what she meant. I did not ask. She left when her maid found her. She said she had enjoyed the conversation. I believe she meant it.",
                "happy", 0.83,
                "She returned to talk. Learned about each other. She said she enjoyed it.",
                new[] { "Elara", "friendship", "conversation", "hope", "memories" }),

            Entry(userId, now.AddDays(-70),
                "Winter Coming, Shop Goes Cold",
                "The cold has come properly now. I stuffed the gap under the east wall with rags but the stall is miserable in the morning and customers move quickly rather than browse. Winter is hard on trade. People buy only what they need and nothing more, and they need less than in autumn when everyone is ordering for feast days. My breath makes clouds inside the shop. Vesper sleeps on top of the receipts box for warmth. I should lay in a proper supply of firewood but the price has gone up again. I think about summer evenings when the market is loud and warm and people stop to look and sometimes buy. I think about those ten minutes of conversation. I am keeping warm on very small fires.",
                "sad", -0.41,
                "Winter has arrived. Cold shop, slow trade. Kept warm by small memories.",
                new[] { "winter", "cold", "slow trade", "Elara", "memories" }),

            Entry(userId, now.AddDays(-63),
                "A Man Tried to Cheat Me",
                "A merchant from out of town tried to return two yards of dyed wool claiming it had faded. The dye was perfectly good. I know my dye. He had treated the cloth incorrectly — probably left it in wet conditions — and the fault was his. He argued loudly enough that other customers began to look. I gave him back half his money just to end the scene and he left looking satisfied, which made me furious. Master Henry says I should have held my ground. He is right. I was tired and cold and my patience was thin and I chose peace over principle. It cost me four pence and a great deal of pride. I will not do that again.",
                "stressed", -0.62,
                "A dishonest customer caused a scene and I gave in. Cost me money and pride.",
                new[] { "dishonest customer", "dispute", "shop", "pride" }),

            Entry(userId, now.AddDays(-55),
                "I Gave a Coin to the Church",
                "I gave more than I could easily spare to the church collection this Sunday, which Father Edmund received with such visible gratitude that I felt guilty for my reasons. I did not give it out of pure piety. I gave it partly hoping that God, who sees and knows everything apparently, might look favourably on the situation with Lady Elara. I know this is not how prayer or charity is supposed to work. I know the Church does not operate as a sort of celestial trade arrangement. Father Edmund would have stern words for me if he understood the ledger behind my generosity. I said an honest prayer afterward, which helped somewhat, and then walked home stepping carefully around the ice.",
                "anxious", -0.21,
                "Donated to the church. Partly sincere, partly a bargain with God about Elara.",
                new[] { "church", "donation", "prayer", "Elara", "guilt" }),

            Entry(userId, now.AddDays(-47),
                "She Brought Me Something",
                "I do not know what to do with this. She came to the stall this morning and she brought me something — a small jar of preserved plums from their kitchen, she said, because last time we spoke she had mentioned that winter made everything taste grey and she thought I might appreciate something sweet. I stood there holding a jar of plums that Lady Elara brought me and I could not speak for a moment. I thanked her. She seemed pleased that I was pleased. She stayed perhaps five minutes, not much more. But she thought of me. Outside of the market, in her own house, she thought of me and decided to bring me something. I have the plums on the shelf next to Vesper. I am not sure I can eat them.",
                "happy", 0.97,
                "She brought me preserved plums as a gift. She thought of me outside of market day.",
                new[] { "Elara", "gift", "plums", "joy", "hope" }),

            Entry(userId, now.AddDays(-39),
                "Thinking of Leaving",
                "I have been thinking about leaving. Not in any decided way, just turning the idea over. There is a cloth merchant in the eastern city who has written twice about a partnership. Good trade, access to Italian routes, a real future. I could go. I have no family here, no binding obligations. I could leave this town and this market and the sight of Lord Aldric's manor in the distance and I could build something better somewhere else. Somewhere no one knows me as simply the cloth merchant. This seems like wisdom. It also seems like cowardice dressed as wisdom. I have not replied to the letter. I keep finding reasons not to.",
                "anxious", -0.31,
                "Considering leaving town for a business opportunity. Cannot bring myself to reply.",
                new[] { "leaving", "opportunity", "cowardice", "Elara", "future" }),

            Entry(userId, now.AddDays(-30),
                "I Am Staying",
                "I wrote to the eastern merchant and declined the partnership. I told him my business here was established and the timing was not right. Both true enough. The real reason: I am not ready to leave. The shop is mine and my father's before me and I have built it into something worth having. And she is here. For another year and a half she is here and I will not spend that time somewhere else pretending I made the rational choice. Let me be honest in this book if nowhere else. I am staying because of Elara. I am staying because of a year and a half of mornings at the market and perhaps one day she will come to my stall again and bring preserved plums and stay more than five minutes.",
                "calm", 0.55,
                "Declined the partnership offer. Staying for the shop — and for her.",
                new[] { "decision", "staying", "Elara", "honesty", "future" }),

            Entry(userId, now.AddDays(-21),
                "The Best Cloth Day of My Life",
                "I must record this properly. Today was the finest single day of trade I have had in my career. A nobleman passing through town on his way to the coast required a complete order — travelling cloth, good linen, two bolts of dyed wool for his household. My best stock, my honest prices, my full knowledge of fabric on display. He shook my hand at the end of it. He said I was the finest cloth merchant he had encountered north of the river and he would send others to me. I earned more today than in the past three weeks combined. I sat in the empty stall after close and I felt — for the first time in a long while — that I am exactly where I am supposed to be and doing exactly what I was meant to do.",
                "happy", 0.89,
                "My best trade day ever. A nobleman complimented my craft and will recommend me.",
                new[] { "best day", "trade", "nobleman", "pride", "craft" }),

            Entry(userId, now.AddDays(-12),
                "She Asked About My Name",
                "She came again. I have lost count of how many times now. She asked me today what my full name was. Not just Edmund, but my proper name. Edmund of where, which family. I told her. I told her my father's name and his trade and that I had been in this town my whole life. She said she had supposed I must have come from somewhere more interesting, given what I knew about cloth. I said that cloth itself was the interesting part — it had travelled more than I had. She laughed. That sound again. She said that was a very good answer. Then she said she had to go. Before she left she asked if I would still be at the market next Tuesday. I said I would. I always am.",
                "excited", 0.88,
                "She asked my full name and family. She laughed. She asked if I would be here Tuesday.",
                new[] { "Elara", "name", "laughter", "Tuesday", "hope" }),

            Entry(userId, now.AddDays(-4),
                "Tuesday",
                "She came on Tuesday. She did not buy anything and neither of us pretended she had come to buy anything. We talked for half an hour. Her maid waited at a distance with the particular patience of someone who has done this before. We talked about the spring market, about whether Godwin's cheap cloth was finally losing him customers (it is), about a book of maps she had been reading that described trade routes across the eastern sea. She said she sometimes wished she could follow one of those routes. I said that I sometimes thought the same. She looked at me with an expression I could not quite read and said she believed me. I believe her too. I do not know what comes next. I only know that on Tuesdays I am the happiest man in this town.",
                "happy", 0.96,
                "She came on Tuesday just to talk. Half an hour. She wishes she could travel. So do I.",
                new[] { "Elara", "Tuesday", "conversation", "maps", "happiness" }),
        };
    }

    private static JournalEntry Entry(
        Guid userId,
        DateTime date,
        string title,
        string content,
        string emotion,
        double sentiment,
        string summary,
        string[] tags)
    {
        var ts = date;
        return new JournalEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            EncryptedContent = content,
            Emotion = emotion,
            SentimentScore = sentiment,
            Summary = summary,
            Tags = JsonSerializer.Serialize(tags),
            CreatedAt = ts,
            UpdatedAt = ts
        };
    }
}
