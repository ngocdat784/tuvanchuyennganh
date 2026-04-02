using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CareerOrientationAPI.Controllers
{
    // ================= SESSION MEMORY =================
    public class SessionMemory
    {
        public int? CurrentMajorId { get; set; }
        public string? LastQuestionType { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public static class SessionStore
    {
        public static Dictionary<string, SessionMemory> Sessions = new();
    }
    [ApiController]
    [Route("api/dialogflow")]
    public class AIController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AIController(AppDbContext db)
        {
            _db = db;
        }

        // ======================================================
        // 🔁 TỪ ĐIỂN ALIAS NGÀNH HỌC
        // ======================================================
        private readonly Dictionary<string, string> MajorAliases = new()
        {
            // 1. Công nghệ thông tin
            { "cntt", "công nghệ thông tin" },
            { "it", "công nghệ thông tin" },
            { "may tinh", "công nghệ thông tin" },
            { "lap trinh", "công nghệ thông tin" },

            // 2. Truyền thông đa phương tiện
            { "ttdpt", "truyền thông đa phương tiện" },
            { "da phuong tien", "truyền thông đa phương tiện" },
            { "media", "truyền thông đa phương tiện" },
            { "truyen thong so", "truyền thông đa phương tiện" },

            // 3. Quản trị kinh doanh
            { "qtkd", "quản trị kinh doanh" },
            { "kinh doanh", "quản trị kinh doanh" },
            { "business", "quản trị kinh doanh" },

            // 4. Marketing & Digital Marketing
            { "marketing", "marketing & digital marketing" },
            { "digital marketing", "marketing & digital marketing" },
            { "mkt", "marketing & digital marketing" },
            { "quang cao", "marketing & digital marketing" },

            // 5. Data Science & Big Data
            { "data science", "data science & big data" },
            { "big data", "data science & big data" },
            { "khoa hoc du lieu", "data science & big data" },
            { "ai", "data science & big data" },

            // 6. Công nghệ phần mềm
            { "ktpm", "công nghệ phần mềm" },
            { "phan mem", "công nghệ phần mềm" },
            { "software", "công nghệ phần mềm" },

            // 7. Tài chính – Ngân hàng – Kế toán
            { "tai chinh", "tài chính – ngân hàng – kế toán" },
            { "ngan hang", "tài chính – ngân hàng – kế toán" },
            { "ke toan", "tài chính – ngân hàng – kế toán" },
            { "finance", "tài chính – ngân hàng – kế toán" },

            // 8. Tâm lý học
            { "tam ly", "tâm lý học" },
            { "psychology", "tâm lý học" },
            { "tham van", "tâm lý học" },

            // 9. Kỹ thuật điều khiển & Tự động hóa
            { "tu dong hoa", "kỹ thuật điều khiển & tự động hóa" },
            { "robot", "kỹ thuật điều khiển & tự động hóa" },
            { "iot", "kỹ thuật điều khiển & tự động hóa" },

            // 10. Ngôn ngữ & Sư phạm
            { "su pham", "ngôn ngữ & sư phạm" },
            { "ngon ngu", "ngôn ngữ & sư phạm" },
            { "giang day", "ngôn ngữ & sư phạm" },
            { "tieng anh", "ngôn ngữ & sư phạm" }
        };

       // ======================================================
// 🌐 DIALOGFLOW WEBHOOK
// ======================================================
[HttpPost("webhook")]
public async Task<IActionResult> Webhook([FromBody] JsonElement body)
{
    try
    {
        var queryResult = body.GetProperty("queryResult");
        string userText = queryResult.GetProperty("queryText").GetString() ?? "";

        // ===== 🧠 LẤY SESSION ID =====
        string sessionPath = body.GetProperty("session").GetString() ?? "";
        string sessionId = sessionPath.Split('/').Last();

        if (!SessionStore.Sessions.ContainsKey(sessionId))
            SessionStore.Sessions[sessionId] = new SessionMemory();

        var session = SessionStore.Sessions[sessionId];
        // ======================================================
// 🌐 GIỚI THIỆU WEBSITE LOVE FUTURE
// ======================================================
if (IsWebsiteQuestion(userText))
{
    return Ok(new
    {
        fulfillmentText =
            "🌟 **Love Future** là nền tảng tư vấn định hướng nghề nghiệp " +
            "dành cho học sinh THPT.\n\n" +
            "🎯 Website giúp bạn:\n" +
            "- Khám phá ngành học phù hợp với bản thân\n" +
            "- Tìm hiểu lộ trình học tập của từng ngành\n" +
            "- Biết được cơ hội nghề nghiệp sau khi ra trường\n" +
            "- Gợi ý các trường đại học phù hợp\n\n" +
            "💡 Love Future được xây dựng để đồng hành cùng bạn " +
            "trong việc lựa chọn tương lai học tập và nghề nghiệp nhé 😊"
    });
}


       

        // ======================================================
        // 🔁 ƯU TIÊN CONTEXT SWITCH (BẺ LÁI)
        // ======================================================
        bool isContextSwitch = IsContextSwitch(userText);

        // ======================================================
        // 👋 CHÀO HỎI (CHỈ KHI KHÔNG PHẢI CONTEXT SWITCH)
        // ======================================================
        if (!isContextSwitch && IsGreeting(userText))
        {
            return Ok(new
            {
                fulfillmentText =
                    "👋 Chào bạn nha! 😊\n" +
                    "Mình là trợ lý tư vấn định hướng ngành học cho học sinh THPT.\n\n" +
                    "Bạn có thể hỏi mình như:\n" +
                    "- Lộ trình học ngành CNTT\n" +
                    "- Ngành marketing học gì\n" +
                    "- Em thích máy tính thì nên học ngành nào"
            });
        }

        // ===== 🎯 XÁC ĐỊNH LOẠI CÂU HỎI =====
        string detectedType = DetectMajorQuestionType(userText);
        // ======================================================
// 📚 HỎI CÓ BAO NHIÊU / CÓ NHỮNG NGÀNH GÌ
// ======================================================
if (IsMajorListQuestion(userText))
{
    return Ok(new
    {
        fulfillmentText = BuildMajorListIntroResponse()
    });
}


       // ======================================================
// 🔍 TÌM NGÀNH TỪ CÂU HỎI
// ======================================================
Major? major = await FindMajorFromUserText(userText);

// 👉 Nếu không tìm thấy ngành → dùng ngành đã nhớ trong session
if (major == null && session.CurrentMajorId.HasValue)
{
    major = await _db.Majors
        .FirstOrDefaultAsync(m => m.MajorId == session.CurrentMajorId.Value);
}

// ======================================================
// 🧑‍💼 XỬ LÝ TƯ VẤN VIÊN (CHỈ KHI KHÔNG CÓ NGÀNH)
// ======================================================
if (major == null && IsCounselorQuestion(userText))
{
    return Ok(new
    {
        fulfillmentText =
            "👩‍💼👨‍💼 Để tìm hiểu về các **tư vấn viên**, " +
            "bạn hãy vào mục **Đội ngũ tư vấn** nha 😊"
    });
}

// ======================================================
// ❌ VẪN KHÔNG CÓ NGÀNH → HỎI LẠI
// ======================================================
if (major == null)
{
    return Ok(new
    {
        fulfillmentText =
            "🤔 Mình chưa xác định được **ngành học** bạn đang quan tâm.\n" +
            "Bạn hãy nói rõ tên ngành hoặc lĩnh vực nhé (ví dụ: CNTT, Marketing, Tâm lý học…)."
    });
}


        // ===== 🧠 QUYẾT ĐỊNH QUESTION TYPE (FIX BẺ LÁI) =====
        string questionType;

        if (isContextSwitch && session.LastQuestionType != null)
        {
            questionType = session.LastQuestionType;
        }
        else if (detectedType != "unknown")
        {
            questionType = detectedType;
        }
        else if (session.LastQuestionType != null)
        {
            questionType = session.LastQuestionType;
        }
        else
        {
            questionType = "description";
        }

        // ===== 💾 GHI NHỚ SESSION =====
        session.CurrentMajorId = major.MajorId;
        session.LastQuestionType = questionType;
        session.LastUpdated = DateTime.Now;

        string response = questionType switch
        {
            "description" => BuildDescriptionResponse(major),
            "difficulty" => BuildDifficultyResponse(major),
            "career" => BuildCareerResponse(major),
            "traits" => await BuildTraitsResponse(major),
            "university" => await BuildUniversitySuggestionResponse(major),
            _ => await BuildRoadmapResponse(major)
        };

        return Ok(new { fulfillmentText = response });
    }
    catch (Exception ex)
    {
        return Ok(new
        {
            fulfillmentText = $"❌ Lỗi hệ thống: {ex.Message}"
        });
    }
}



        [HttpGet("webhook")]
        public IActionResult WebhookGet()
        {
            return Ok("Dialogflow webhook is running ✅");
        }
        private bool IsContextSwitch(string text)
{
    text = NormalizeText(text);

    string[] patterns =
    {
        "thi sao",
        "con",
        "con thi sao",
        "the",
        "the thi sao",
        "van",
        "van thi sao"
    };

    return patterns.Any(p => text.Contains(p));
}
private string BuildMajorListIntroResponse()
{
    return
        "🎓 Hiện nay có **rất nhiều ngành nghề khác nhau**, tiêu biểu như:\n\n" +
        "• Công nghệ thông tin 💻\n" +
        "• Marketing & Digital Marketing 📊\n" +
        "• Tâm lý học 🧠\n" +
        "• Quản trị kinh doanh 🏢\n" +
        "• Data Science & AI 📈\n" +
        "• Ngôn ngữ & Sư phạm 🌍\n\n" +
        "👉 Nếu bạn **quan tâm ngành nào**, cứ nói tên ngành, mình sẽ **tư vấn chi tiết từ A–Z** cho bạn nhé 😊";
}

// ======================================================
// 📚 NHẬN DIỆN CÂU HỎI: CÓ BAO NHIÊU / CÓ NHỮNG NGÀNH GÌ
// ======================================================
private bool IsMajorListQuestion(string userText)
{
    userText = NormalizeText(userText);

    string[] keywords =
    {
        "co bao nhieu nganh",
        "co nhung nganh nao",
        "co nganh gi",
        "co nhung nganh gi",
        "tu van duoc nganh nao",
        "he thong co nganh nao",
        "nganh nghe nao",
        "co nhieu nganh khong"
    };

    return keywords.Any(k => userText.Contains(k));
}

// ======================================================
// 🧑‍💼 NHẬN DIỆN CÂU HỎI VỀ TƯ VẤN VIÊN
// ======================================================
private bool IsCounselorQuestion(string userText)
{
    userText = NormalizeText(userText);

    // 🔑 Từ khóa cố định
    string[] keywords =
    {
        "tu van vien",
        "co van",
        "chuyen gia tu van",
        "doi ngu tu van",
        "thay",
        "co",
        "anh",
        "chi"
    };

    // ✔️ Có từ khóa
    if (keywords.Any(k => userText.Contains(k)))
        return true;

    // ✔️ Có dấu hiệu hỏi về người (họ tên)
    // Ví dụ: "anh nguyen van a", "co le thi b"
    var namePattern = new Regex(@"\b(anh|chi|thay|co)\s+[a-z]{2,}(\s+[a-z]{2,}){1,2}\b");
    if (namePattern.IsMatch(userText))
        return true;

    return false;
}
// ======================================================
// 🌐 NHẬN DIỆN CÂU HỎI VỀ WEBSITE LOVE FUTURE
// ======================================================
private bool IsWebsiteQuestion(string userText)
{
    userText = NormalizeText(userText);

    string[] keywords =
    {
        "love future",
        "website nay",
        "trang web nay",
        "web nay",
        "he thong nay",
        "nen tang nay",
        "lovefuture"
    };

    return keywords.Any(k => userText.Contains(k));
}

        // ======================================================
        // 👋 KIỂM TRA CHÀO HỎI
        // ======================================================
        private bool IsGreeting(string text)
{
    text = NormalizeText(text);

    // 👉 Điều kiện độ dài (RẤT QUAN TRỌNG)
    if (text.Length > 20)
        return false;

    string[] greetings =
    {
        "chao",
        "chao ban",
        "xin chao",
        "hello",
        "hi",
        "hey",
        "ad oi",
        "ban oi",
        "cho minh hoi",
        "cho em hoi"
    };

    return greetings.Any(g => text == g || text.StartsWith(g));
}

// ======================================================
// 🔍 TÌM NGÀNH – ƯU TIÊN KEYWORDS DB
// ======================================================
private async Task<Major?> FindMajorFromUserText(string userText)
{
    if (string.IsNullOrWhiteSpace(userText))
        return null;

    userText = NormalizeText(userText);

    var majors = await _db.Majors.ToListAsync();

    // ==================================================
    // 🔥 TẦNG 0 – ALIAS (MATCH THEO TỪ, KHÔNG SUBSTRING)
    // ==================================================
    var words = userText
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .ToList();

    foreach (var alias in MajorAliases)
    {
        // alias.Key có thể là 1 hoặc nhiều từ (vd: "ngan hang")
        var aliasWords = alias.Key
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // 👉 TẤT CẢ từ của alias phải xuất hiện ĐỘC LẬP
        bool isMatch = aliasWords.All(aw => words.Contains(aw));

        if (isMatch)
        {
            return majors.FirstOrDefault(m =>
                NormalizeText(m.Name) == NormalizeText(alias.Value));
        }
    }

    // ==================================================
    // 🔥 TẦNG 1 – MATCH THEO KEYWORDS DB (CHUẨN NHẤT)
    // ==================================================
    foreach (var major in majors)
    {
        if (string.IsNullOrWhiteSpace(major.Keywords))
            continue;

        var keywordList = major.Keywords
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim());

        foreach (var kw in keywordList)
        {
            if (userText.Contains(kw))
                return major;
        }
    }

    // ==================================================
    // 🔥 TẦNG 2 – MATCH TOKEN (DỰ PHÒNG)
    // ==================================================
    var tokens = ExtractKeywords(userText);
    if (!tokens.Any())
        return null;

    foreach (var major in majors)
    {
        if (string.IsNullOrWhiteSpace(major.Keywords))
            continue;

        var keywordList = major.Keywords
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim());

        foreach (var token in tokens)
        {
            if (keywordList.Any(k => k == token))
                return major;
        }
    }

    return null;
}



        // ======================================================
// 🧹 CHUẨN HÓA TEXT (lowercase + bỏ dấu TV + ký tự thừa)
// ======================================================
private string NormalizeText(string text)
{
    if (string.IsNullOrWhiteSpace(text))
        return string.Empty;

    // 1️⃣ lowercase
    text = text.ToLower();

    // 2️⃣ bỏ dấu tiếng Việt
    var normalized = text.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();

    foreach (var c in normalized)
    {
        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        {
            sb.Append(c);
        }
    }

    text = sb.ToString().Normalize(NormalizationForm.FormC);

    // 3️⃣ xoá ký tự đặc biệt
    text = Regex.Replace(text, @"[^\w\s]", "");

    // 4️⃣ chuẩn hoá khoảng trắng
    text = Regex.Replace(text, @"\s+", " ");

    return text.Trim();
}


        private List<string> ExtractKeywords(string text)
{
    text = NormalizeText(text);

    var stopWords = new HashSet<string>
    {
        "lo", "trinh", "nganh", "hoc",
        "ra", "sao", "the", "nao",
        "la", "gi", "cho", "minh", "hoi",
        "minh", "muon", "tu", "van",
        "ve", "em", "anh", "chi",
        "hien", "tai", "co", "bao", "nhieu"
    };

    return text
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Where(t => t.Length > 3 && !stopWords.Contains(t))
        .ToList();
}


      // ======================================================
// 🧾 BUILD ROADMAP RESPONSE
// ======================================================
private async Task<string> BuildRoadmapResponse(Major major)
{
    var roadmap = await _db.Roadmaps
        .Where(r => r.MajorId == major.MajorId)
        .Include(r => r.Stages)
            .ThenInclude(s => s.Items)
        .FirstOrDefaultAsync();

    if (roadmap == null)
        return $"📘 Ngành {major.Name} hiện chưa có lộ trình học chi tiết.";

    var sb = new StringBuilder();

    sb.AppendLine($"🎓 Ngành: {major.Name}");
    sb.AppendLine();
    sb.AppendLine("🧭 Lộ trình học tập:");

    foreach (var stage in roadmap.Stages.OrderBy(s => s.StageOrder))
    {
        sb.AppendLine();
        sb.AppendLine($"🔹 {stage.StageTitle}");

        foreach (var item in stage.Items.OrderBy(i => i.ItemOrder))
        {
            sb.AppendLine($"- {item.ItemTitle}: {item.Content}");
        }
    }

    return sb.ToString();
}
private async Task<string> BuildUniversitySuggestionResponse(Major major)
{
    using var client = new HttpClient();

    // ⚠️ đổi URL cho đúng môi trường của bạn
    string apiUrl =
        $"http://localhost:5086/api/university/by-major/{major.MajorId}";

    var response = await client.GetAsync(apiUrl);

    if (!response.IsSuccessStatusCode)
        return $"🏫 Hiện chưa có dữ liệu trường đại học cho ngành {major.Name}.";

    var json = await response.Content.ReadAsStringAsync();
    var universities = JsonSerializer.Deserialize<List<University>>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (universities == null || !universities.Any())
        return $"🏫 Chưa tìm thấy trường phù hợp với ngành {major.Name}.";

    var sb = new StringBuilder();
    sb.AppendLine($"🏫 Các trường đào tạo ngành **{major.Name}**:");
    sb.AppendLine();

    foreach (var uni in universities.Take(5))
    {
        sb.AppendLine($"🎓 {uni.Name}");
    }

    sb.AppendLine();
    sb.AppendLine("👉 Bạn muốn mình so sánh các trường này không?");

    return sb.ToString();
}


private string DetectMajorQuestionType(string userText)
{
    userText = NormalizeText(userText);
    if (userText.Contains("to chat") ||
    userText.Contains("to chat gi") ||
    userText.Contains("phu hop") ||
    userText.Contains("co hop khong") ||
    userText.Contains("can ky nang gi") ||
    userText.Contains("can to chat gi"))
    return "traits";

    if (userText.Contains("truong") ||
        userText.Contains("dai hoc") ||
        userText.Contains("hoc o dau") ||
        userText.Contains("nen hoc truong nao"))
        return "university";

    if (userText.Contains("kho") ||
        userText.Contains("ap luc") ||
        userText.Contains("de khong"))
        return "difficulty";

    if (userText.Contains("ra truong") ||
        userText.Contains("lam gi") ||
        userText.Contains("nghe gi") ||
        userText.Contains("viec lam"))
        return "career";

    if (userText.Contains("la gi") ||
        userText.Contains("gioi thieu"))
        return "description";

    if (userText.Contains("lo trinh") ||
        userText.Contains("hoc gi") ||
        userText.Contains("hoc nhu the nao"))
        return "roadmap";

    return "unknown"; // ✅ QUAN TRỌNG NHẤT
}

// ======================================================
// 🧠 TRẢ LỜI: TỐ CHẤT PHÙ HỢP NGÀNH
// ======================================================
private async Task<string> BuildTraitsResponse(Major major)
{
    var traits = await _db.MajorTraits
        .Where(t => t.MajorId == major.MajorId)
        .ToListAsync();

    if (traits == null || !traits.Any())
    {
        return $"🧠 Ngành {major.Name} hiện chưa có dữ liệu về tố chất yêu cầu.";
    }

    var sb = new StringBuilder();

    sb.AppendLine($"🧠 Để học tốt ngành **{major.Name}**, bạn nên có:");
    sb.AppendLine();

    foreach (var trait in traits)
    {
        sb.AppendLine($"✅ {trait.Title}");
    }

    sb.AppendLine();
    sb.AppendLine("👉 Nếu bạn muốn, mình có thể giúp bạn **đánh giá mức độ phù hợp của bản thân** với ngành này nhé 😊");

    return sb.ToString();
}

// ======================================================
// 🧾 TRẢ LỜI: NGÀNH LÀ GÌ
// ======================================================
private string BuildDescriptionResponse(Major major)
{
    return
        $"📘 Ngành {major.Name} là \n\n" +
        $"{major.Description ?? "Chưa có mô tả ngành."}\n\n" +
        $"{major.Details ?? ""}";
}


private string BuildDifficultyResponse(Major major)
{
    return
        $"⚡ Độ khó của ngành {major.Name} thì\n\n" +
        $"{major.Difficulty ?? "Hiện chưa có đánh giá độ khó cho ngành này."}";
}


// ======================================================
// 💼 TRẢ LỜI: RA TRƯỜNG LÀM GÌ
// ======================================================
private string BuildCareerResponse(Major major)
{
    return
        $"💼 Cơ hội nghề nghiệp ngành {major.Name} thì\n\n" +
        $"{major.Career ?? "Chưa có thông tin nghề nghiệp cho ngành này."}";
}


    }
}

