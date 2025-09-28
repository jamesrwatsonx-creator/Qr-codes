# Custom Blocklists

Flink provides a powerful **Blocklist mechanism** to prevent malicious or unwanted URLs from being shortened.  
By default, Flink already ships with a lot of regularly updated blocklist, but you can also create and maintain 
your own custom blocklists for additional control. This customization guide shows you how.

---

## The official Flink Blocklist

You can find the official list here: [blocklist.txt](https://gitlab.com/rtraceio/web/flink/-/blob/main/blocklist.txt).

This list contains domains and URLs that have been reported as malicious, scam-related, or otherwise harmful by:
- Official Flink instances
- The Flink community

Before starting a new blocklist, I'd like to encourage you to provide entries for the Blocklist above. This way the
whole Flink community can benefit from your findings.

---

## Creating Your Own Custom Blocklist

You may want to enforce stricter rules on your own instance. For example,
you might decide to block links to **privacy-invading software** or specific services.

Let’s walk through an example where we block Google services.

### Step 1: Create a Blocklist File

Flink expects blocklists to be served as plain text over **HTTP(S)**.  
The easiest way to host such a file is to create a repository on [GitLab](https://gitlab.com), [GitHub](https://github.com), [Bitbucket](https://bitbucket.org), [Codeberg](https://codeberg.org), or any similar git hoster.
Alternatively, a simple webserver accessible by Flink shipping the blocklist works too.

Here’s a minimal example blocklist file:

```txt
[Adblock Plus 2.0]
! Version: 11
! Title: Anti-Google-Blocklist
! Last modified: 26 Sep 2025 20:00 UTC
! Expires: 1 days (update frequency)
!

||google.at^ 
||google.com^
||youtube.com^
```

### Step 2: add Your Blocklist to Flink

Follow the [SpamProtection Configuration](https://gitlab.com/rtraceio/web/flink/-/blob/main/docs/config/config.md?ref_type=heads#spam-protection-configuration)
to see how to add your blocklist. Depending on your deployment/installation, you might need to use Environment-Variables to extend the array of Blocklists,
or change the [appsettings.json](https://gitlab.com/rtraceio/web/flink/-/blob/main/Flink/appsettings.json) file yourself.

### Step 3: Extend

You can add as many additional domains as you like.
The format follows the [Adblock Plus filter syntax](https://adblockplus.org/en/filter-cheatsheet) which makes it easy to manage and reuse existing community blocklists.

Example additions:
```txt
||facebook.com^
||instagram.com^
||tiktok.com^
```

## Best Practices

- Keep it updated: Use the `! Expires` header to control how often Flink refreshes the blocklist.
- Be specific: Block only what you really need to avoid breaking legitimate use cases.
- Combine lists: You can maintain multiple blocklists (e.g., ads, scams, trackers) and merge them for your instance.
- Test before deployment: Ensure your blocklist does not unintentionally block essential services.
